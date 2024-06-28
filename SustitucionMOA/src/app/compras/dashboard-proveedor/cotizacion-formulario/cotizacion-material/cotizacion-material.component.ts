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
import { CotizacionPosicionDto, GuardarCotizacion } from '../../../../modelos/cotizacionDto';
import { Dropdown } from 'primeng/dropdown';
import { PosicionCompra } from '../../../solp-compra';


@Component({
    selector: 'app-cotizacion-material',
    templateUrl: 'cotizacion-material.component.html',
    styleUrls: ['./cotizacion-material.component.css'],
    providers: [ComprasService, UsuarioService]
})
export class CotizacionMaterialComponent extends ListBaseComponent implements OnInit, OnChanges {

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("tabla")
    protected tabla: Table;
    archivos = new Array<File>()

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
    visualizarMensajeDeModificacion: boolean = false;
    hoy: Date = new Date();
    displayPlazo: boolean;
    cotizacionPosicion: CotizacionPosicionDto;
    cantidadSolicitada: number;

    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }
    ngOnChanges(changes: SimpleChanges): void {
        this.getCombos();
        this.mostrarMensajeNoRespetaCondiciones();
    }

    ngOnInit() {
        this.getCombos();
        this.setComboMoneda();
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
        this.mostrarMensajeNoRespetaCondiciones()
    }

    public mostrarMensajeNoRespetaCondiciones() {
        for (var i = 0; i < this.posicionesCompra.length; i++) {
            this.visualizarMensajeDeModificacion =
                this.validarCambios(this.posicionesCompra[i].Posiciones.CotizacionPosicion, this.posicionesCompra[i].Id);
            if (this.visualizarMensajeDeModificacion) {
                break;
            }
        }
    }

    public get esTipoMaterial(): boolean {
        return this.peticion.TipoPosicionCodigo == "MATERIALES";
    }

    public get esTipoServicio(): boolean {
        return this.peticion.TipoPosicionCodigo == "SERVICIO";
    }

    eliminarArchivo(esAdjuntoNuevo: boolean, archivo: any) {
        this.confirmationService.confirm({
            message: '¿Está seguro de que desea eliminar el archivo?',
            accept: () => {
                esAdjuntoNuevo ? this.eliminarAdjuntoNuevo(archivo) : this.eliminarAdjuntoGuardado(archivo)
            },
            reject: () => {

            }
        });
    }

    //elimno el archivo, llamar al servicio de eliminacion
    eliminarAdjuntoNuevo(archivo): void {
        var indice = this.archivos.indexOf(archivo)
        this.archivos.splice(indice, 1)
    }

    eliminarAdjuntoGuardado(archivo): void {
        var indice = this.peticion.Cotizacion.ArchivosCotizacion.indexOf(archivo)
        this.peticion.Cotizacion.ArchivosCotizacion.splice(indice, 1)
    }

    descargarArchivo(archivo): void {
        if (archivo.Id != undefined) {
            this.service.DescargarArchivo(archivo.Id)
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

    public onSelectMoneda(cotizacion: any, peticionId, event) {
        cotizacion.MonedaCodigo = event.value.Codigo;
        cotizacion.Moneda_Id = event.value.Id;
        //var moneda = this.combos.Moneda.filter(x => x.CodigoDescripcion.includes(cotizacion.MonedaCodigo.Codigo))[0];
        //if (moneda == undefined) {
        //    return this.floatMsgService.setErrorMsg("Debe seleccionar una moneda válida");
        //}
        //this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.CotizacionPosicion.Moneda_Id = moneda.Id;
    }

    public onSelectUnidad(cotizacion: any, peticionId, event) {
        cotizacion.UnidadComprasDescripcion = event.value.Codigo;
        cotizacion.UnidadDeMedida_Id = event.value.Id;
        //var unidad = this.combos.Unidades.filter(x => x.Descripcion.includes(cotizacion.UnidadMedidaDescripcion.Descripcion))[0];
        //if (unidad == undefined) {
        //    return this.floatMsgService.setErrorMsg("Debe seleccionar una unidad de medida válida");
        //}
        //this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.CotizacionPosicion.UnidadDeMedida_Id = unidad.Id;
        this.mostrarMensajeNoRespetaCondiciones();
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

    uploadHandler(filesUpload: any) {
        var archivoWeb = filesUpload["files"].reduce((sum, file) => sum + file.size, 0);
        this.archivos = filesUpload["files"];
        if (archivoWeb > 10000000) {
            if (this.archivos.length > 0) {
                this.eliminarAdjuntoNuevo(this.archivos[this.archivos.length - 1])
            }
            this.floatMsgService.setErrorMsg("El archivo adjuntado no debe superar los 10Mb")
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
                        this.setComboMoneda();
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

    public setComboMoneda(): void {
        //Obtengo todas las opciones de los autocomplete
        if (this.combos != undefined) {
            this.monedaCompras = this.combos.Moneda;
            //this.unidades = this.combos.Unidades;
        }
    }

    public getUnidades(posicionesCompra: any): any[] {
        return posicionesCompra.Posiciones.UnidadesDeMedida;
    }

    public calcularValorNeto(cotizacion: CotizacionPosicionDto, peticionId: number) {
        if (cotizacion.Precio > 0 && cotizacion.Cantidad > 0) {
            cotizacion.PrecioTotal = cotizacion.Precio * cotizacion.Cantidad;
            this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.CotizacionPosicion.PrecioTotal = cotizacion.PrecioTotal;
        }
    }

    public validarCambios(cotizacion: any, peticionId: number) {
        this.visualizarMensajeDeModificacion = false;
        var respuesta = false;
        if (cotizacion.Cantidad > 0) {
            respuesta = this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.Cantidad != cotizacion.Cantidad;

        }
        if (!respuesta && cotizacion.UnidadMedida != undefined && cotizacion.UnidadMedida.Id != 0) {
            respuesta = this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.UnidadId != cotizacion.UnidadMedida.Id
        }
        return respuesta;
    }

    calcularFechaEntrega(cotizacion: CotizacionPosicionDto, peticionId: number) {
        let fechaNueva = new Date();
        if (cotizacion.PlazoDeEntrega > 0) {
            fechaNueva.setDate(fechaNueva.getDate() + cotizacion.PlazoDeEntrega);
            this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.CotizacionPosicion.FechaDeEntrega = fechaNueva;
        }
    }

    calcularPlazo(cotizacion: CotizacionPosicionDto, peticionId: number) {
        let fechaNueva = new Date();
        let fechaOriginal = new Date();
        if (cotizacion.FechaDeEntrega != null) {
            fechaNueva = cotizacion.FechaDeEntrega;
        } else {
            cotizacion.FechaDeEntrega = fechaOriginal;
        }
         // Calcula la diferencia en milisegundos entre las dos fechas
        const diferenciaEnMilisegundos = fechaNueva.getTime() - fechaOriginal.getTime();

        // Convierte la diferencia en días
        const dias = Math.ceil(diferenciaEnMilisegundos / (1000 * 60 * 60 * 24));
        this.posicionesCompra.filter(x => x.Id == peticionId)[0].Posiciones.CotizacionPosicion.PlazoDeEntrega = dias;
    }

    public getCotizacion() {
        this.crearCotizacionPosicion();
        var coti = {
            CotizacionId: this.peticion.CotizacionId,
            PeticionOfertaUsuarioId: this.peticion.Id,
            CotizacionPosiciones: this.cotizaciones,
            ObservacionEconomica: this.peticion.ObservacionEconomica,
            RespetaMateriales: this.peticion.RespetaMateriales,
            ObservacionTecnica: "",
            ArchivosNuevos: this.archivos,
            ArchivosGuardados: this.peticion.Cotizacion.ArchivosCotizacion != null ? this.peticion.Cotizacion.ArchivosCotizacion.map(x => { return { Id: x.Id } }) : null,
            ArchivosTipo: this.peticion.Cotizacion.ArchivosCotizacion != null ? this.peticion.Cotizacion.ArchivosCotizacion.map(x => { return { FileKey: x.FileKey } }) : null,
        }
        return coti;
    }

    public crearCotizacionPosicion() {

        return this.cotizaciones = this.posicionesCompra.map(cotizacion => {
            return {
                PeticionDeOfertaSolpPosicionId: cotizacion.Posiciones.Id,
                Posicion: cotizacion.Posiciones.Indice,
                SOLP: cotizacion.Posiciones.NroSolp,
                Cantidad: cotizacion.Posiciones.CotizacionPosicion.Cantidad != null ? cotizacion.Posiciones.CotizacionPosicion.Cantidad : 0,
                Precio: cotizacion.Posiciones.CotizacionPosicion.Precio != null ? cotizacion.Posiciones.CotizacionPosicion.Precio : 0,
                MonedaId: cotizacion.Posiciones.CotizacionPosicion.Moneda_Id != 0 ?
                    cotizacion.Posiciones.CotizacionPosicion.Moneda_Id : 0,
                UnidadDeMedidaId: cotizacion.Posiciones.CotizacionPosicion.UnidadDeMedida_Id != 0 ?
                    cotizacion.Posiciones.CotizacionPosicion.UnidadDeMedida_Id : 0,
                FechaDeEntrega: cotizacion.Posiciones.CotizacionPosicion.FechaDeEntrega != undefined ?
                    cotizacion.Posiciones.CotizacionPosicion.FechaDeEntrega : null,
                UnidadMedida: cotizacion.Posiciones.CotizacionPosicion.UnidadComprasDescripcion,
                monedaCompras: cotizacion.Posiciones.CotizacionPosicion.MonedaCodigo,
                CantidadSubpos: cotizacion.Posiciones.Cantidad,
                UnidadDeMedidaSubpos: cotizacion.Posiciones.UnidadId,
                NoDisponible: cotizacion.Posiciones.CotizacionPosicion.NoDisponible,
                FechaDeVigencia: cotizacion.Posiciones.CotizacionPosicion.FechaDeVigencia != undefined ?
                cotizacion.Posiciones.CotizacionPosicion.FechaDeVigencia : null,
                PrimerPlazoDeOferta: cotizacion.Posiciones.CotizacionPosicion.PrimerPlazoDeOferta != null ? cotizacion.Posiciones.CotizacionPosicion.PrimerPlazoDeOferta : 0,
                PrimeraCantidad: cotizacion.Posiciones.CotizacionPosicion.PrimeraCantidad != null ? cotizacion.Posiciones.CotizacionPosicion.PrimeraCantidad : 0,
                SegundoPlazoDeOferta: cotizacion.Posiciones.CotizacionPosicion.SegundoPlazoDeOferta != null ? cotizacion.Posiciones.CotizacionPosicion.SegundoPlazoDeOferta : 0,
                SegundaCantidad: cotizacion.Posiciones.CotizacionPosicion.SegundaCantidad != null ? cotizacion.Posiciones.CotizacionPosicion.SegundaCantidad : 0,
                TercerPlazoDeOferta: cotizacion.Posiciones.CotizacionPosicion.TercerPlazoDeOferta != null ? cotizacion.Posiciones.CotizacionPosicion.TercerPlazoDeOferta : 0,
                TerceraCantidad: cotizacion.Posiciones.CotizacionPosicion.TerceraCantidad != null ? cotizacion.Posiciones.CotizacionPosicion.TerceraCantidad : 0,
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

    public ObtenerArchivos() {
        return this.archivos;
    }

    generarZipPliego(idSolp) {
        this.blockUI.start('Generando ')
        this.service.descargarZipPliego(idSolp)
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

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                result.FileDownloadName
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = result.FileDownloadName;
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);
                            this.blockUI.stop();
                            return false;
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            )
    }

    noDisponible(cotizacion: CotizacionPosicionDto) {
        cotizacion.Cantidad = null;
        cotizacion.Moneda = null;
        cotizacion.MonedaCodigo = null;
        cotizacion.Moneda_Id = null;
        cotizacion.Precio = null;
        cotizacion.UnidadDeMedida_Id = null;
        cotizacion.UnidadMedida = null;
        cotizacion.PlazoDeEntrega = null;
        cotizacion.PrecioTotal = null;
        cotizacion.FechaDeVigencia = null;
        cotizacion.PrimerPlazoDeOferta  = null;
        cotizacion.PrimeraCantidad  = null;
        cotizacion.SegundoPlazoDeOferta = null;
        cotizacion.SegundaCantidad = null;
        cotizacion.TercerPlazoDeOferta = null;
        cotizacion.TerceraCantidad = null;
    }

    onEditarCelda(cotizacion: any, campo: string, valorInicial: any) {
        if (cotizacion[campo] === valorInicial) {
            cotizacion[campo] = ''; // Limpia el valor si es igual al valorInicial
        }
    }

    onReestablecerValor(cotizacion: any, campo: string) {
        if (cotizacion[campo] === '' || cotizacion[campo] === null) {
            cotizacion[campo] = 0; // Restablece a cero si está en blanco
        }
    }

    validarNumero(event: any) {
        const inputValue = event.target.value;        
        if (isNaN(inputValue) || inputValue < 0) {
          event.target.value = 0; // Borra el valor si es negativo
        }
      }

      abrirPlazoDeOferta(cotizacionPosicion: CotizacionPosicionDto, cantidad: number){
        this.displayPlazo = true;
        this.cotizacionPosicion = cotizacionPosicion;
        this.cantidadSolicitada = cantidad;
      }

      cerrarPlazo() {
        this.displayPlazo = false;
    }
}
