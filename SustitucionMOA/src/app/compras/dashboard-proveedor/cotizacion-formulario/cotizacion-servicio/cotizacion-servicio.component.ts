import { Component, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild, EventEmitter } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ComprasService } from '../../../compras.service';
import { UsuarioService } from '../../../../usuario/usuario.service';
import { ListBaseComponent } from '../../../../common/base-components/list-base-component';
import { SecurityService } from '../../../../common/services/SecurityService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { NavService } from '../../../../common/services/NavService';
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { PeticionDeOfertaDto, PeticionDeOfertaSolpPosicionDto } from '../../../../modelos/peticion-de-oferta-model';
import { SelectItem } from 'ng2-select';
import { ValorTotalPorMoneda } from '../../../solp/solp';
import { CotizacionHoraDto, CotizacionPosicionDto, GuardarCotizacion } from '../../../../modelos/cotizacionDto';


@Component({
    selector: 'app-cotizacion-servicio',
    templateUrl: 'cotizacion-servicio.component.html',
    styleUrls: ['./cotizacion-servicio.component.css'],
    providers: [ComprasService, UsuarioService]
})
export class CotizacionServicioComponent extends ListBaseComponent implements OnInit, OnChanges {

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("tabla")
    protected tabla: Table;
    archivosTecnico = new Array<File>()
    archivosEconomico = new Array<File>()

    @Input() peticion: PeticionDeOfertaDto;
    @Input() posicionesCompra: PeticionDeOfertaSolpPosicionDto[];
    observaciones: string;
    combos: any;
    monedaCompras: SelectItem[];
    unidades: any[];
    @Input() esFinalizado: boolean;

    @Input('locale') es: any;
    cotizaciones: GuardarCotizacion[];
    valorTotalPorMoneda: ValorTotalPorMoneda[];
    displayCotizacionCreada: boolean;
    visualizarMensajeDeModificacion: boolean;
    hoy: Date = new Date();
    cotizacionHora: CotizacionHoraDto
    index: number = 0;


    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }
    ngOnChanges(changes: SimpleChanges): void {
        this.getCombos();
    }

    ngOnInit() {
        this.getCombos();
        this.setCombos();
        this.es = {
            firstDayOfWeek: 1,
            dayNames: ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"],
            dayNamesShort: ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"],
            dayNamesMin: ["D", "L", "M", "X", "J", "V", "S"],
            monthNames: ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"],
            monthNamesShort: ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"],
            today: 'Hoy',
            clear: 'Borrar'
        }
        console.log(this.peticion, "peticion")
        var tieneDatos = this.peticion.Cotizacion.CotizacionesHoras.some(x => x.Gremio != 'UOCRA');
        this.index = tieneDatos ? 1 : 0;
        if (!tieneDatos) {
            this.agregarRow();
        }
    }

    agregarFilaDefault() {
        if (this.peticion.Cotizacion.CotizacionesHoras)
            this.cotizacionHora = { Cotizacion_Id: 0, CantidadPersonas: 0, Categoria: "", Gremio: "", HorasExtras: 0, HorasNocturnas: 0, HorasNormales: 0 };
        this.peticion.Cotizacion.CotizacionesHoras.push(this.cotizacionHora)
    }

    eliminarArchivo(esAdjuntoNuevo: boolean, archivo: any, esTecnico) {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar el archivo?',
            accept: () => {
                esAdjuntoNuevo ? this.eliminarAdjuntoNuevo(archivo, esTecnico) : this.eliminarAdjuntoGuardado(archivo)
            },
            reject: () => {

            }
        });

    }

    //elimno el archivo, llamar al servicio de eliminacion
    eliminarAdjuntoNuevo(archivo, esTecnico): void {
        if (esTecnico) {
            var indice = this.archivosTecnico.indexOf(archivo)
            this.archivosTecnico.splice(indice, 1)
        } else {
            var indice = this.archivosEconomico.indexOf(archivo)
            this.archivosEconomico.splice(indice, 1)
        }
    }

    eliminarAdjuntoGuardado(archivo): void {
        var indice = this.peticion.Cotizacion.ArchivosCotizacion.indexOf(archivo)
        this.peticion.Cotizacion.ArchivosCotizacion.splice(indice, 1)
    }

    descargarArchivo(archivo): void {
        if (archivo.id != undefined) {

            this.service.DescargarArchivo(archivo.id)
                .subscribe(
                    (result) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        }
                        else {
                            var byteArray = new Uint8Array(result.FileContents);
                            var blob = new Blob([byteArray], {
                                type: "application/octet-stream",
                            });

                            this.downloadArchivoLocal(blob, result.FileDownloadName);
                        }
                    },
                    (error) => {
                        this.spinnerSmallComponent.hideIt();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                )
        }
        else {
            this.downloadArchivoLocal(archivo, archivo.name);

        }
    }

    public onSelectMoneda(cotizacion: any, peticionId) {
        var moneda = this.combos.Moneda.filter(x => x.CodigoDescripcion.includes(cotizacion.MonedaCodigo.Codigo))[0];
        this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.CotizacionPosicion.Moneda_Id = moneda.Id;
    }

    public onSelectUnidad(cotizacion: any, peticionId) {
        var unidad = this.combos.Unidades.filter(x => x.Descripcion.includes(cotizacion.UnidadMedidaDescripcion.Descripcion))[0];
        this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.CotizacionPosicion.UnidadDeMedida_Id = unidad.Id;
    }
    private downloadArchivoLocal(blob: Blob, nombreArchivo: string): void {
        if (window.navigator.msSaveOrOpenBlob) {
            // IE11
            window.navigator.msSaveOrOpenBlob(
                blob,
                nombreArchivo
            );
        } else {
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement("a");
            document.body.appendChild(link);
            link.href = url;
            link.download = nombreArchivo;
            link.click();
            setTimeout(function () {
                window.URL.revokeObjectURL(url);
            }, 0);
            return;
        }
    }


    public onValueChangeObservaciones(event: Event): void {
        const value = (event.target as any).value;
        this.observaciones = value;
    }

    salir() {
        this.navService.navegarSeccion("/compras/dashboardProveedor");
    }

    uploadHandler(filesUpload: any, esTecnico) {
        if (esTecnico) {
            var archivoWeb = filesUpload["files"].reduce((sum, file) => sum + file.size, 0);
            this.archivosTecnico = filesUpload["files"];
            if (archivoWeb > 10000000) {
                this.floatMsgService.setErrorMsg("El archivo adjuntado no debe superar los 10Mb")
            }
            console.log(this.archivosTecnico, "archivosT")
        } else {
            var archivoWeb = filesUpload["files"].reduce((sum, file) => sum + file.size, 0);
            this.archivosEconomico = filesUpload["files"];
            if (archivoWeb > 10000000) {
                this.floatMsgService.setErrorMsg("El archivo adjuntado no debe superar los 10Mb")
            }
        }

    }

    buscarCombo(event, type) {
        switch (type) {
            case 'MONEDA COMPRAS':
                this.monedaCompras = this.combos.Moneda.filter(x => x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;
            case 'UNIDAD MEDIDA':
                this.unidades = this.combos.Unidades.filter(x => x.Descripcion.toLowerCase().includes(event.query.toLowerCase()));
                break;
            default:
                break;
        }
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.combos = result;
                        this.setCombos();
                        console.log(this.combos, "combos")
                        //this.combos.flagSolpFinalizada = this.flagSolpFinalizada;                       
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    public setCombos(): void {
        //Obtengo todas las opciones de los autocomplete
        if (this.combos != undefined) {
            this.monedaCompras = this.combos.Moneda;
            this.unidades = this.combos.Unidades;
        }
    }

    public calcularValorNeto(cotizacion: CotizacionPosicionDto, peticionId: number) {
        if (cotizacion.Precio != 0 && cotizacion.Cantidad != 0) {
            cotizacion.PrecioTotal = cotizacion.Precio * cotizacion.Cantidad;
            this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.CotizacionPosicion.PrecioTotal = cotizacion.PrecioTotal;

        }
    }


    public validarCambios(cotizacion: any, peticionId: number) {
        this.visualizarMensajeDeModificacion = false;
        if (cotizacion.Cantidad != 0) {
            this.visualizarMensajeDeModificacion = this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.Cantidad != cotizacion.Cantidad;
            return this.visualizarMensajeDeModificacion;
        }
        if (cotizacion.UnidadMedidaDescripcion != undefined) {
            this.visualizarMensajeDeModificacion = this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.UnidadId != cotizacion.UnidadMedidaDescripcion.Id
            return this.visualizarMensajeDeModificacion;
        }
    }


    public getCotizacion() {
        this.crearCotizacionPosicion();
        var coti = {
            CotizacionId: this.peticion.CotizacionId,
            PeticionOfertaUsuarioId: this.peticion.Id,
            CotizacionPosiciones: this.cotizaciones,
            ObservacionEconomica: this.peticion.ObservacionEconomica,
            ObservacionTecnica: this.peticion.ObservacionTecnica,
            ArchivosNuevos: this.archivosEconomico,
            ArchivosTecnico: this.archivosTecnico,
            RespetaServicios: this.peticion.RespetaServicios,
            RespetaMateriales: this.peticion.RespetaMateriales,
            ArchivosGuardados: this.peticion.Cotizacion.ArchivosCotizacion != null ? this.peticion.Cotizacion.ArchivosCotizacion.map(x => { return { Id: x.Id } }) : null,
            CotizacionesHoras: this.index == 0 ? this.peticion.Cotizacion.CotizacionesHoras.filter(x => x.Gremio == 'UOCRA') : this.peticion.Cotizacion.CotizacionesHoras.filter(x => x.Gremio != 'UOCRA')
        }
        return coti;
    }

    public crearCotizacionPosicion() {

        return this.cotizaciones = this.posicionesCompra.map(cotizacion => {
            return {
                PeticionDeOfertaSolpPosicionId: cotizacion.Posiciones.Id,
                Posicion: cotizacion.Posiciones.Indice,
                Cantidad: cotizacion.Posiciones.CotizacionPosicion.Cantidad,
                Precio: cotizacion.Posiciones.CotizacionPosicion.Precio,
                MonedaId: cotizacion.Posiciones.CotizacionPosicion.Moneda_Id != 0 ?
                    cotizacion.Posiciones.CotizacionPosicion.Moneda_Id : 0,
                UnidadDeMedidaId: cotizacion.Posiciones.CotizacionPosicion.UnidadDeMedida_Id != 0 ?
                    cotizacion.Posiciones.CotizacionPosicion.UnidadDeMedida_Id : 0,
                FechaDeEntrega: cotizacion.Posiciones.CotizacionPosicion.FechaDeEntrega != undefined ?
                    cotizacion.Posiciones.CotizacionPosicion.FechaDeEntrega : null,
                UnidadMedida: cotizacion.Posiciones.CotizacionPosicion.UnidadComprasDescripcion,
                monedaCompras: cotizacion.Posiciones.CotizacionPosicion.MonedaCodigo,
            };
        });
    }

    calcularValorTotalPorMoneda() {
        this.valorTotalPorMoneda = new Array<ValorTotalPorMoneda>();
        const monedas = this.posicionesCompra.map(item => item.Posiciones.CotizacionPosicion.MonedaCodigo).filter((value, index, self) => self.indexOf(value) === index);
        monedas.forEach(moneda => {
            let valorTotal = this.posicionesCompra.filter(p => p.Posiciones.CotizacionPosicion.MonedaCodigo).reduce((sum, current) => sum + current.valorTotal, 0);
            if (isNaN(valorTotal)) {
                valorTotal = 0;
            }
            this.valorTotalPorMoneda.push({ moneda, valorTotal } as ValorTotalPorMoneda);
        });
    }

    public ObtenerArchivosTecnicos() {
        return this.archivosTecnico;
    }

    public ObtenerArchivosEconomicos() {
        return this.archivosEconomico;
    }

    agregarRow() {
        if (this.peticion.Cotizacion.CotizacionesHoras
            .find(x => x.CantidadPersonas == 0 && x.HorasNocturnas == 0 && x.Gremio == ""
                && x.HorasExtras == 0 && x.Categoria == "" && x.HorasNormales == 0) == null) {
            this.cotizacionHora = { Cotizacion_Id: 0, CantidadPersonas: 0, Categoria: "", Gremio: "", HorasExtras: 0, HorasNocturnas: 0, HorasNormales: 0 };
            this.peticion.Cotizacion.CotizacionesHoras.push(this.cotizacionHora)
        } else {
            this.floatMsgService.setErrorMsg("Debe completar el registro anterior para agregar uno nuevo")
        }

    }

    eliminarRow(index: number) {
        if (this.peticion.Cotizacion != null) {
            this.peticion.Cotizacion.CotizacionesHoras.splice(index, 1);
        }

    }


    cambiarTab(e: MouseEvent, index) {
        e.stopPropagation();
        if (this.index == index) {
            return;
        }
        e.preventDefault();
        var mensaje = index == 0 ? 'Otros' : 'UOCRA ' + '¿Quiere continuar?';
        this.confirmationService.confirm({
            message: 'Esta acción eliminará los datos cargados en la solapa ' + mensaje,
            accept: () => {
                this.eliminarDatosCotizacionHoras(index);
                this.index = index;
            },
            reject: () => {

            }
        });

    }

    eliminarDatosCotizacionHoras(index) {

        this.peticion.Cotizacion.CotizacionesHoras.forEach(element => {
            element.CantidadPersonas = 0,
                element.HorasNocturnas = 0,
                element.Categoria = index == 1 ? element.Categoria : "",
                element.HorasNormales = 0
            element.Gremio = index == 1 ? element.Gremio : ""
        });
    }

    validarDatosCotizacionHoras(e: MouseEvent, index) {
        e.preventDefault();
        var mostrarMensaje = this.peticion.Cotizacion.CotizacionesHoras
            .some(x => (
                x.Gremio == "UOCRA" && (x.CantidadPersonas > 0 ||
                    x.HorasNocturnas > 0 || x.HorasExtras > 0 || x.HorasNormales > 0)) ||
                (x.Gremio != "UOCRA" && (x.Categoria != "" || x.CantidadPersonas > 0 ||
                    x.HorasNocturnas > 0 || x.HorasExtras > 0 || x.HorasNormales > 0)));

        if (mostrarMensaje) {
            this.cambiarTab(e, index)
        }
    }


}
