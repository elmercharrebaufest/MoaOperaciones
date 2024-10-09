import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ComprasService } from '../compras.service';
import { UsuarioService } from '../../usuario/usuario.service';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { NavService } from '../../common/services/NavService';
import { ModalService } from '../../common/services/ModalService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { AltaNuevoProveedor, EnvioSolpCompra, PosicionCompra, SolpCompraDto, SolpProveedorDto, SolpSubposicionDto } from '../solp-compra';
import { RegistroInfoDto } from '../../modelos/registro-info';
import { first } from 'rxjs/operators';

@Component({
    selector: 'app-peticion-de-oferta-formulario',
    templateUrl: './peticion-de-oferta-formulario.component.html',
    styleUrls: ['./peticion-de-oferta-formulario.component.css', '../compras.component.css'],
    providers: [ComprasService, UsuarioService]
})
export class PeticionDeOfertaFormularioComponent extends ListBaseComponent implements OnInit {

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("tabla")
    protected tabla: Table;

    solpCompraDto: SolpCompraDto;
    posicionCompra: PosicionCompra[];
    solpSubposicionDto: SolpSubposicionDto[];
    solpProveedorDto: SolpProveedorDto[];
    envioSolpCompra: EnvioSolpCompra[];
    altaNuevoProveedor: AltaNuevoProveedor;
    cuitProveedor: string = "";
    mailProveedor: string;
    razonSocialProveedor: string;
    proveedoresValidos: string;
    proveedoresInvalidos: string;
    proveedoresNoSugeridos: string;
    archivos = new Array<File>()
    posicionDeInicioInsert: number = 0;
    nroPeticion: any;
    displayPeticionCreada: boolean = false;
    display: boolean = false;
    displayFinalizar: boolean = false;
    proveedores: any[] = new Array();
    proveedoresSeleccionados: AltaNuevoProveedor[] = new Array();
    proveedorSeleccionado: any;
    observaciones: string;
    displayAltaProveedor: boolean = false;
    displayProvCreado: boolean = false;
    datoProveedor: string;
    displayRegistroInfo: boolean = false;
    TodasPosicionesSeleccionadas: boolean = false;
    pliegoDeGeneralidades: boolean = false;
    esTipoPOMultiple: boolean = false;
    plazoDeEntrega: Date;
    hayPosicionesPendientes(): boolean { return this.solpCompraDto.PosicionCompras.some(x => x.Cantidad > 0) };
    posicionesPendientes(): PosicionCompra[] { return this.solpCompraDto.PosicionCompras.filter(x => x.Cantidad > 0) };
    plazoDeEntregaFechaMin: Date;

    locale = {
        firstDayOfWeek: 0,
        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
        dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        today: 'Hoy',
        clear: 'Borrar'
    };

    constructor(protected service: ComprasService, protected usuarioService: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);



    }

    ngOnInit() {
        this.solpCompraDto = { Id: null, NroSolp: null, PosicionCompras: null, TipoPosicionCodigo: "" };
        this.altaNuevoProveedor = { CUIT: null, Mail: null, RazonSocial: null };

        this.route.params.subscribe(params => {
            let ids = params['id'];

            if (!isNaN(ids)) {
                this.obtenerSolpCompras(ids);
                this.esTipoPOMultiple = false;


            } else {
                this.obtenerPosicionesMultipleCompras(ids);
                this.esTipoPOMultiple = true;

            }
        });

        const validos = this.filtrarProveedores(this.proveedores, 'validos');
        const invalidos = this.filtrarProveedores(this.proveedores, 'invalidos');
        const noSugeridos = this.filtrarProveedores(this.proveedores, 'no sugeridos');

        this.proveedoresValidos = this.mostrarProveedores(validos);
        this.proveedoresInvalidos = this.mostrarProveedores(invalidos);
        this.proveedoresNoSugeridos = this.mostrarProveedores(noSugeridos);

        this.setPlazoDeEntregaFechaMin();
    }

    obtenerSolpCompras(id) {
        try {
            this.blockUI.start('Cargando...');
            this.subscription = this.service.obtenerSolpCompras(id).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.procesarResultado(result);

                    }
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();
                });
        } catch (e) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    private procesarResultado(result: any) {
        this.solpCompraDto = result.data;
        //verificar tipo de pos y si es serv setear selected en true
        if (this.esTipoServicio) {
            this.solpCompraDto.PosicionCompras.forEach(x => x.Selected = true);
        }

        if (this.esTipoPOMultiple) {
            this.TodasPosicionesSeleccionadas = true;
            this.seleccionarTodo();
        }

        for (var i = 0; i < this.solpCompraDto.PosicionCompras.length; i++) {
            for (var j = 0; j < this.solpCompraDto.PosicionCompras[i].ProveedoresCompras.length; j++) {
                this.proveedores.push(this.solpCompraDto.PosicionCompras[i].ProveedoresCompras[j]);
            }
        }
        const validos = this.filtrarProveedores(this.proveedores, 'VALIDO');
        const invalidos = this.filtrarProveedores(this.proveedores, 'INVALIDO');
        const noSugeridos = this.filtrarProveedores(this.proveedores, 'NOSUGERIDO');

        this.proveedoresValidos = this.mostrarProveedores(validos);
        this.proveedoresInvalidos = this.mostrarProveedores(invalidos);
        this.proveedoresNoSugeridos = this.mostrarProveedores(noSugeridos);

        if (this.solpCompraDto.PlazoDeOfertaTentativo) {
            this.plazoDeEntrega = new Date(this.solpCompraDto.PlazoDeOfertaTentativo);
        }
    }

    obtenerPosicionesMultipleCompras(ids) {
        try {
            this.blockUI.start('Cargando...');
            this.subscription = this.service.obtenerPosicionesMultipleCompras(ids).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.procesarResultado(result);

                    }
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();
                });
        } catch (e) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }



    public get esTipoMaterial(): boolean {
        return this.solpCompraDto.TipoPosicionCodigo == "MATERIALES" || (this.solpCompraDto.PosicionCompras != null && this.solpCompraDto.PosicionCompras[0].TipoPosicionCodigo == "MATERIALES");
    }

    public get esTipoServicio(): boolean {
        return this.solpCompraDto.TipoPosicionCodigo == "SERVICIO";
    }

    public get esMaterialCatalogado(): boolean {
        if (this.esTipoMaterial) {
            return this.solpCompraDto.PosicionCompras.some(c => c.MaterialComprasCodigo != undefined);
        }
    }

    //elimno el archivo, llamar al servicio de eliminacion
    eliminarAdjuntoNuevo(archivo): void {
        console.log(archivo);
        var indice = this.archivos.indexOf(archivo)
        this.archivos.splice(indice, 1)
    }

    uploadHandler(filesUpload: any): void {
        var archivoWeb = filesUpload["files"].reduce((sum, file) => sum + file.size, 0);
        this.archivos = filesUpload["files"];
        if (archivoWeb > 10000000) {
            if (this.archivos.length > 0) {
                this.eliminarAdjuntoNuevo(this.archivos[this.archivos.length - 1])
            }
            this.floatMsgService.setErrorMsg("El archivo adjuntado no debe superar los 10Mb");
        }
    }

    eliminarArchivo(archivo: any) {
        this.confirmationService.confirm({
            message: '¿Está seguro de que desea eliminar el archivo?',
            accept: () => {
                this.eliminarAdjuntoNuevo(archivo)
            },
            reject: () => {

            }
        });
    }

    filtrarProveedores(proveedores: SolpProveedorDto[], tipoFiltro: string): SolpProveedorDto[] {
        return proveedores.filter((proveedor) => proveedor.TipoFiltroProveedorSolpCodigo === tipoFiltro);
    }

    mostrarProveedores(proveedores: SolpProveedorDto[]): string {
        return proveedores.map((proveedor) => proveedor.RazonSocial).join(', ');
    }

    searchProveedor(event) {
        try {
            this.subscription = this.service.listarProveedores(event.query).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.proveedores = result.data;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    selectProveedor(event) {
        try {
            if (!this.proveedoresSeleccionados.some(e => e.Id === event.Id)) {
                this.proveedoresSeleccionados.push(event);
            }
            this.proveedorSeleccionado = null;
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
        }
    }

    eliminarProveedor(event, proveedor) {
        this.proveedoresSeleccionados = this.proveedoresSeleccionados.filter(x => x.Id !== proveedor.Id);
    }

    onShowFinalizarDialog() {
        if (!this.solpCompraDto.PosicionCompras.some(x => x.Selected == true)) {
            this.floatMsgService.setErrorMsg("No selecciono una posicion");
            return;
        }

        if (this.proveedoresSeleccionados == null || this.proveedoresSeleccionados.length == 0) {
            this.floatMsgService.setErrorMsg("No selecciono un Proveedor");
            return;
        }
        this.confirmationService.confirm({
            header: "¡Ultimo Paso!",
            acceptLabel: "SI, CONFIRMAR",
            rejectLabel: "VOLVER",
            message: 'Esta a punto de enviar la peticion de oferta. <b>¿Desea confirmar?</b>',
            accept: () => {
                this.guardarPeticion();
            },
            reject: () => {
            }
        });
    }

    guardarPeticion() {
        this.blockUI.start("Grabando...");
        try {
            let envio: EnvioSolpCompra = {
                Adjuntos: this.archivos,
                Observacion: this.observaciones,
                PosIds: this.solpCompraDto.PosicionCompras.filter(x => x.Selected == true).map(a => a.Id),
                SolpId: this.solpCompraDto.Id,
                UsuarioIds: this.proveedoresSeleccionados.map(a => a.Id),
                AdjuntoPliego: this.pliegoDeGeneralidades,
                PlazoDeEntrega: this.plazoDeEntrega,
            }
            this.subscription = this.service.GrabarPeticion(envio).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.nroPeticion = result.data.IdEntidad;
                        this.displayPeticionCreada = true;
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();

                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh

    }

    public onValueChangeObservaciones(event: Event): void {
        const value = (event.target as any).value;
        this.observaciones = value;
    }

    salir() {
        this.navService.navegarSeccion("/compras/dashboardComprador");

    }

    descargarArchivo(archivo) {
        this.downloadArchivoLocal(archivo, archivo.name);
    }

    private downloadArchivoLocal(blob: Blob, nombreArchivo: string): void {
        if (window.navigator.msSaveOrOpenBlob) {            // IE11            
            window.navigator.msSaveOrOpenBlob(blob, nombreArchivo);
        } else {
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement("a");
            document.body.appendChild(link); link.href = url;
            link.download = nombreArchivo; link.click();
            setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
            return;
        }
    }

    abrirPopupProveedor() {
        this.displayAltaProveedor = true;
    }

    salirPopupProveedor() {
        this.displayAltaProveedor = false;
    }

    agregarProveedor(event) {
        this.selectProveedor(event.proveedorDto);
    }

    mostrarRegistroInfo() {
        this.displayRegistroInfo = true;
    }

    salirPopupRegistroInfo() {
        this.displayRegistroInfo = false;
    }

    seleccionarTodo() {
        if (this.TodasPosicionesSeleccionadas) {
            this.solpCompraDto.PosicionCompras
                .filter(x =>
                    !(x.Cantidad <= 0)
                    && !x.Selected // por checkSelectAllIfNeeded, prevenir ciclos
                )
                .map(pos => pos.Selected = true);
        } else {
            this.solpCompraDto.PosicionCompras.map(pos => pos.Selected = false);
        }
    }

    checkSelectAllIfNeeded(): void {
        this.TodasPosicionesSeleccionadas = this.posicionesPendientes().every(x => x.Selected);
    }

    setPlazoDeEntregaFechaMin(): void {
        this.plazoDeEntregaFechaMin = this.dateDiasDesdeHoy(0); // hoy
    }

    get mostrarAlertaFechaInferior10Dias(): boolean {
        if (!this.plazoDeEntrega) { return false; }
        let limite = this.dateDiasDesdeHoy(10);
        return this.plazoDeEntrega < limite;
    }

    dateDiasDesdeHoy(cantidadDeDiasParaSumar: number): Date {
        cantidadDeDiasParaSumar *= 24; // convertir días a horas
        cantidadDeDiasParaSumar *= 60; // convertir a minutos
        cantidadDeDiasParaSumar *= 60; // convertir a segundos
        cantidadDeDiasParaSumar *= 1000; // se representa en milisegundos;

        let returnValue: Date = new Date(Date.now() + (cantidadDeDiasParaSumar))
        returnValue.setHours(0, 0, 0, 0);

        return returnValue;
    }
}