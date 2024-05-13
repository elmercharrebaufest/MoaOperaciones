import { Component, Renderer, OnDestroy, ViewChild, ChangeDetectorRef, HostListener } from '@angular/core';
import { Router } from "@angular/router";
import { Seccion } from './../common/models/seccion';
import { LayoutService } from './layout.service';
import { SessionDataService } from './../common/services/SessionDataService';
import { NavService } from './../common/services/NavService';
import { ModalService } from './../common/services/ModalService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { MensajeModalComponent } from './../common/view-child/mensaje-modal/mensaje-modal.component';
import { LoginGuard } from './../common/security/login-guard';
import { ok } from 'assert';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { BuscadorComponent } from './../common/shared-components/buscador/buscador.component'
declare var $: any;

@Component({
    selector: 'app-layout',
    templateUrl: `layout.component.html`,
    providers: [LayoutService]
})

export class LayoutComponent implements OnDestroy {
    @BlockUI() blockUI: NgBlockUI;
    titulo: string;
    username: string;
    nombre: string;
    proveedor: string;
    permisos: Array<string>;
    granosFlag: string;
    granosSelected: string;
    tipoUsuario: string;
    noticias: any;
    quantityCommunications:number = 0;
    modalHeader: any;
    showLiquidaciones = false;
    showComprobantes = false;
    showDetalle = false;
    showFlete = false;
    showNoticias = false;
    showFleteProcedencia = false;
    modalInfoLiquidacion: any;
    modalLiquidacionContrato: any;
    modalLiquidacionSecuencia: any;
    modalLiquidacionComprobante: any;
    modalInfoComprobante: any;
    modalInfoDetalle: any;
    modalInfoFlete: any;
    modalNoticias: any;
    modalInfoFleteProcedencia: any;
    modalInfoContratoProcedencia: any;
    modalFooter: any;
    seccionList: Array<Seccion>;
    tieneSecciones: boolean = false;
    seccionActive: string;
    menuActive: string = 'home';
    mensajeError = '';
    mensajeInfo = '';
    mensajeSuccess = '';
    mensajeErrorModal = '';
    subscription: any;
    textoTooltip: string = '';
    textoTooltip2: string = '';
    seccionesVisitadas: string;
    auxiliarSeccionesVisitadas: string = '';
    menuSmall: boolean = false;
    showComunicaciones: boolean = true;
    comunicacionesIsOpen: boolean = false;

    @ViewChild("myModal") modal: any;

    @ViewChild("mensajeModal")
    protected mensajeModalComponent: MensajeModalComponent;

    @HostListener('window:resize', ['$event'])
        onResize(event) {
        this.validarSreen();
    }

    @HostListener('document:click', ['$event'])
    clickout(event) {
        const allowedIds = ["notificationBell", "UserNotification", "openNotification", "notificationClose", "notificationOpenSmall","notificationCloseSmall" ];
        const notAllowedIds = ["unread-button", "comunicacion-target", "body-message-comunicaciones"];
        const targetId = event.target.id || (event.target.parentElement ? event.target.parentElement.id : null);

        if (notAllowedIds.includes(event.target.id)) {
            return;
        }
        
        if (allowedIds.includes(targetId)) {
            this.showComunicaciones = true;
            this.comunicacionesIsOpen = true;
        } else {
            this.showComunicaciones = false;
            this.comunicacionesIsOpen = false;
        }

        if (this.showComunicaciones) {
            $("#notificationSmall, #notificationClose").css({ "display" : "block" });
            $("#notificationOpen").css({ "display" : "none" });
            $("#notificationSmall").css({ "right" : "0" });
            $("#coverAll").fadeIn();
        } else {
            $("#notificationClose, #notificationSmall").css({ "display": "none" });
            $("#notificationOpen").css({ "display": "block" });
            $("#notificationSmall").css({ "right" : "-270px" });
            $("#coverAll").fadeOut();
        }

        this.showComunicaciones = true;
    }

    //se porque al usuario comercial se le asigno el nuevo rol de 
    //alta empresa granos y solo deberia acceder desde el listado
    esUsuarioComercial : boolean = false;

    constructor(private service: LayoutService, private sessionDataService: SessionDataService,
        private navService: NavService, private loginGuard: LoginGuard, private router: Router,
        protected renderer: Renderer, private modalService: ModalService, private securityService: SecurityService, private floatMsgService: FloatMsgService,
    private cd: ChangeDetectorRef) {
        
        this.renderer.setElementClass(document.body, 'wrapper', true);

        this.titulo = 'Moa Operaciones';
        this.nombre = sessionStorage.getItem("nombre");
        this.username = sessionStorage.getItem("username");
        this.proveedor = sessionStorage.getItem("proveedor");
        this.granosFlag = sessionStorage.getItem("granosFlag");
        this.granosSelected = sessionStorage.getItem("granosSelected");
        this.tipoUsuario = sessionStorage.getItem("tipoUsuario");
        this.noticias = JSON.parse(sessionStorage.getItem("noticias"));
        this.seccionActive = this.navService.seccionActiveValue;
        this.menuActive = this.navService.menuActiveValue;
        this.seccionesVisitadas = sessionStorage.getItem("seccionesVisitadas");

        sessionDataService.username$.subscribe(
            username => {
                this.username = username;
            });
        sessionDataService.nombre$.subscribe(
            nombre => {
                this.nombre = nombre;
            });
        sessionDataService.proveedor$.subscribe(
            proveedor => {
                this.proveedor = proveedor;
            });
        sessionDataService.granosFlag$.subscribe(
            granosFlag => {
                this.granosFlag = granosFlag;
            });
        sessionDataService.granosSelected$.subscribe(
            granosSelected => {
                this.granosSelected = granosSelected;
            });
        sessionDataService.tipoUsuario$.subscribe(
            tipoUsuario => {
                this.tipoUsuario = tipoUsuario;
            });
        sessionDataService.noticias$.subscribe(
            noticias => {
                this.noticias = noticias;
            });

        sessionDataService.seccionesVisitadas$.subscribe(
            seccionesVisitadas => {
                this.seccionesVisitadas = seccionesVisitadas;
            });

        navService.seccionList$.subscribe(
            seccionList => {
                this.seccionList = seccionList

                this.tieneSecciones = seccionList.length > 0;

                this.cd.detectChanges();
            });
    
        navService.seccionActive$.subscribe(
            seccionActive => {
                this.seccionActive = seccionActive;

                switch (this.seccionActive) {
                    case 'Fijaciones':
                    case 'Ampliaciones':
                    case 'Anulaciones':
                    case 'Vigentes':
                        this.textoTooltip = 'En esta categoría podrás visualizar los negocios concertados, sus fijaciones, ampliaciones y anulaciones.';
                        this.textoTooltip2 = 'Haciendo click en el número de contrato podrás visualizar mayor información sobre el mismo (características, condiciones comerciales, estado del boleto, aplicaciones, calidades, liquidaciones asociadas y pagos).';
                        this.auxiliarSeccionesVisitadas = 'Vigentes';
                        break;
                    case 'Aplicaciones':
                    case 'Formulario':
                    case 'Descargas':
                        this.auxiliarSeccionesVisitadas = 'Descargas';
                        this.textoTooltip = 'En esta categoría podrás visualizar el detalle de tus entregas y la imagen de las cartas de porte correspondientes.';
                        this.textoTooltip2 = 'En la solapa de "Aplicaciones" podrás ver a qué negocio fueron asignadas. Además, en "Formulario" podrás autocompletar tu carta de porte a partir del CTG e imprimirla con el formulario otorgado por AFIP.';
                        break;
                    case 'Pendientes de registro':
                    case 'Pagas':
                    case 'Registrados':
                        this.auxiliarSeccionesVisitadas = 'Registrados';
                        this.textoTooltip = 'En esta categoría podrás visualizar el estado de tus liquidaciones.';
                        this.textoTooltip2 = 'En la solapa de "Aprobadas" podrás ver aquellas están en condiciones de incluirse en el proceso de pagos. En "Observadas" aquellas que se encuentran en proceso de contabilización o que presentan diferencias que impiden su registración. En "Pagas" aquellas ya fueron pagadas.';
                        break;
                    case 'Detalle de pagos':
                    case 'Cuenta Corriente':
                        this.auxiliarSeccionesVisitadas = 'Cuenta Corriente';
                        this.textoTooltip = 'En la solapa "Cuenta Corriente" podrás visualizar los movimientos y el saldo correspondiente.';
                        this.textoTooltip2 = 'En la solapa "Detalle de pago" podrás visualizar los comprobantes que hayan sido cancelados agrupados por número de orden de pago.';
                        break;
                    case 'Emitidos':
                        this.auxiliarSeccionesVisitadas = 'Emitidos';
                        this.textoTooltip = 'En esta categoría podrás visualizar el detalle de tus pagos por número de identificación pudiendo descargar los documentos asociados al pago (comprobantes, orden de pago y certificados de retención).';
                        this.textoTooltip2 = '';
                        break;
                    case 'Vendedor Estado':
                        this.auxiliarSeccionesVisitadas = 'Vendedor Estado';
                        this.textoTooltip = 'En esta categoría podrás consultar si uno o más vendedores están habilitados para operar con nosotros.';
                        this.textoTooltip2 = '';
                        break
                    case 'Mi Situacion Fiscal':
                        this.auxiliarSeccionesVisitadas = 'Mi Situacion Fiscal';
                        this.textoTooltip = 'En esta categoría podrás visualizar el estado de tu perfil impositivo (Estado en SISA, exenciones vigentes/vencidas, cuentas bancarias e inscripción en Ing. Brutos).';
                        this.textoTooltip2 = '';
                        break;
                    case 'Mis Vendedores':
                        this.auxiliarSeccionesVisitadas = 'Mis Vendedores';
                        this.textoTooltip = 'En esta categoría podrás visualizar el perfil impositivo de tus vendedores habilitados (Estado en SISA, exenciones vigentes/vencidas, cuentas bancarias e inscripción en Ing. Brutos).';
                        this.textoTooltip2 = '';
                        break;
                    case 'Vendedores pendientes':
                        this.auxiliarSeccionesVisitadas = 'Vendedores pendientes';
                        this.textoTooltip = 'En esta categoría podrás visualizar aquellos vendedores que se encuentran en proceso de alta y consultar su grado de avance, el estado dela documentación presentada y requerida.';
                        this.textoTooltip2 = '';
                        break;
                    case 'Documentacion':
                        this.auxiliarSeccionesVisitadas = 'Documentacion';
                        this.textoTooltip = 'En esta categoría podrás consultar nuestros datos de contacto, horario de atención, direcciones de envío y documentos útiles para operar (Legajo de Molinos Agro, Documentación de Alta, Cesiones de Pago y mercadería, Certificado de depósito y carta de garantía, tarifas de servicios e instructivos).';
                        this.textoTooltip2 = '';
                        break;
                    case 'Viajes Pendientes':
                    case 'Viajes Facturados':
                    case 'Viajes A Facturar':
                        this.auxiliarSeccionesVisitadas = 'Viajes A Facturar';
                        this.textoTooltip = 'En esta categoría podrás visualizar tus viajes pendientes de proformar, facturar y facturados.';
                        this.textoTooltip2 = '';
                        break;
                    case 'Contacto':
                        this.auxiliarSeccionesVisitadas = 'Contacto';
                        this.textoTooltip = 'En esta categoría podrás contactarte con nosotros y resolver tus dudas o consultas, reclamar pagos y retenciones, y enviar documentación.';
                        this.textoTooltip2 = '';
                        break;
                    case 'Pesificacion':
                        this.auxiliarSeccionesVisitadas = 'Pesificacion';
                        this.textoTooltip = 'En esta categoría podrás pesificar tus negocios en dólares, individual o masivamente, y consultar aquellos negocios pendientes de pesificar.';
                        this.textoTooltip2 = '';
                        break;
                    case 'Gestion':
                        this.auxiliarSeccionesVisitadas = 'Gestion';
                        this.textoTooltip = 'Instructivo Productor: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/echeqProductor.mp4" target="_blank">Click aqui</a>.';
                        this.textoTooltip2 = 'Instructivo Acopiador: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/echeqAcopiador.mp4" target="_blank">Click aqui</a>.';
                        break;
                        case 'Mis Echeq':
                        this.auxiliarSeccionesVisitadas = 'Mis Echeq';
                        this.textoTooltip = 'En esta categoría podrás cargar tus echeqs';
                        this.textoTooltip2 = '';
                        break;
                    case 'Carga de Negocios':
                        this.auxiliarSeccionesVisitadas = 'Carga de Negocios';
                        this.textoTooltip = 'En esta pantalla podrás seleccionar el tipo de negocio que deseas operar.';
                        this.textoTooltip2 = '';//'<a href="https://www.youtube.com/watch?v=NooUcellVgY&list=RDEMb_bDv34i1yX9BKQmQom11w&index=4" target="_blank">click aqui</a>';
                        break;
                    case 'A Precio':
                        this.auxiliarSeccionesVisitadas = 'A Precio';
                        this.textoTooltip = 'Instructivo Corredor: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/Carga Contrato A PRECIO Corredor - MOAOPERACIONES.mp4" target="_blank">click aqui</a>.';
                        this.textoTooltip2 = 'Instructivo Directo: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/DIRECTOS - Carga Contrato a Precio - Moaoperaciones.mp4" target="_blank">click aqui</a>.';
                        break;
                    case 'A Fijar':
                        this.auxiliarSeccionesVisitadas = 'A Fijar';
                        this.textoTooltip = 'Instructivo Corredor: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/Carga Contrato A FIJAR Corredor - MOAOPERACIONES.mp4" target="_blank">click aqui</a>.';
                        this.textoTooltip2 = 'Instructivo Directo: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/DIRECTOS%20-%20Carga%20Contrato%20A%20Fijar%20-%20Moaoperaciones.mp4" target="_blank">click aqui</a>.';
                        break;
                    case 'Fijacion':
                        this.auxiliarSeccionesVisitadas = 'Fijacion';
                        this.textoTooltip = 'Instructivo Corredor: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/Carga%20Fijaci%C3%B3n%20Corredor%20-%20MOAOPERACIONES.mp4" target="_blank">click aqui</a>.';
                        this.textoTooltip2 = 'Instructivo Directo: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/DIRECTOS%20-%20Carga%20Fijaci%C3%B3n%20-%20Moaoperaciones.mp4" target="_blank">click aqui</a>.';
                        break;
                    case 'Alta Masiva':
                        this.auxiliarSeccionesVisitadas = 'Alta Masiva';
                        this.textoTooltip = 'Instructivo Corredor: <a href="https://b2cmoagro.blob.core.windows.net/moaopublic/Carga%20Masiva%20Contratos%20Corredor%20-%20MOAOPERACIONES.mp4" target="_blank">click aqui</a>.';
                        this.textoTooltip2 = '';
                        break;
                    case 'Ingresar certificación':
                        this.auxiliarSeccionesVisitadas = 'Ingresar certificación';
                        this.textoTooltip = '¡Bienvenido! Aquí tienes una guía rápida para utilizar esta página: <br/><br/>' + 
                                            '1. Utiliza los filtros para afinar tu búsqueda.<br/>' + 
                                            '2. La grilla muestra los detalles de las órdenes de compra filtradas.<br/>' +
                                            '3. Las flechas en la primera columna te permiten expandir y ver más información sobre las posiciones de cada orden de compra.<br/>' +
                                            '4. Al seleccionar uno o varios ítems, se activará el botón para certificar las entradas de servicios correspondientes.<br/>';
                        this.textoTooltip2 = '';
                        break;
                    default:
                        this.textoTooltip = '';
                        this.textoTooltip2 = '';
                        break;
                }
                this.cd.detectChanges();
            });
        navService.menuActive$.subscribe(
            menuActive => {
                this.menuActive = menuActive
            });

        modalService.modalHeader$.subscribe(
            modalHeader => {
                this.modalHeader = modalHeader
            });

        modalService.modalInfoLiquidacion.subscribe(
            modalInfoLiquidacion => {
                this.modalInfoLiquidacion = modalInfoLiquidacion
            });

        modalService.modalLiquidacionContrato.subscribe(
            modalLiquidacionContrato => {
                this.modalLiquidacionContrato = modalLiquidacionContrato
            });

        modalService.modalLiquidacionSecuencia.subscribe(
            modalLiquidacionSecuencia => {
                this.modalLiquidacionSecuencia = modalLiquidacionSecuencia
            });

        modalService.modalLiquidacionComprobante.subscribe(
            modalLiquidacionComprobante => {
                this.modalLiquidacionComprobante = modalLiquidacionComprobante
            });

        modalService.modalShowLiquidacion.subscribe(
            modalShowLiquidacion => {
                this.showLiquidaciones = modalShowLiquidacion
            });

        modalService.modalInfoComprobante.subscribe(
            modalInfoComprobante => {
                this.modalInfoComprobante = modalInfoComprobante
            });

        modalService.modalShowComprobante.subscribe(
            modalShowComprobante => {
                this.showComprobantes = modalShowComprobante
            });

        modalService.modalShowNoticia.subscribe(
            modalShowNoticia => {
                this.showNoticias = modalShowNoticia
            });

        modalService.modalInfoDetalle.subscribe(
            modalInfoDetalle => {
                this.modalInfoDetalle = modalInfoDetalle
            });

        modalService.modalShowDetalle.subscribe(
            modalShowDetalle => {
                this.showDetalle = modalShowDetalle
            });

        modalService.modalInfoFlete.subscribe(
            modalInfoFlete => {
                this.modalInfoFlete = modalInfoFlete
            });

        modalService.modalShowFlete.subscribe(
            modalShowFlete => {
                this.setEmptyMsjModal();
                this.showFlete = modalShowFlete
            });

        modalService.modalContratoFleteProcedencia.subscribe(
            modalContratoFleteProcedencia => {
                this.modalInfoContratoProcedencia = modalContratoFleteProcedencia
            });

        modalService.modalInfoFleteProcedencia.subscribe(
            modalInfoFleteProcedencia => {
                this.modalInfoFleteProcedencia = modalInfoFleteProcedencia
            });

        modalService.modalShowFleteProcedencia.subscribe(
            modalShowFleteProcedencia => {
                this.showFleteProcedencia = modalShowFleteProcedencia
            });

        modalService.modalFooter$.subscribe(
            modalFooter => {
                this.modalFooter = modalFooter
            });

        modalService.modalMsjError.subscribe(
            modalMsjError => {
                this.setErrorMsjModal(modalMsjError);
            });

        modalService.modalMsjSuccess.subscribe(
            modalMsjSuccess => {
                this.setSuccessMsjModal(modalMsjSuccess);
            });

        floatMsgService.errorMsj$.subscribe(
            errorMsj => {
                this.setMsjError(errorMsj);
            });

        floatMsgService.infoMsj$.subscribe(
            infoMsj => {
                this.setMsjInfo(infoMsj);
            });

        floatMsgService.successMsj$.subscribe(
            successMsj => {
                this.setMsjSuccess(successMsj);
            });

         this.esUsuarioComercial = this.securityService.tienePermiso('ABM EMPRESAS');
    }

    ngAfterViewInit() {
        this.modalService.modal = this.modal;
        this.mensajeModalComponent = new MensajeModalComponent();
        this.blockUI.stop();
    }

    cerrarSesion() {
        this.sessionDataService.logout();
        return false; // <- Prevent href del a
    }

    isSeccionVisitada(){
        return this.seccionesVisitadas.includes(this.auxiliarSeccionesVisitadas);
    }

    actualizarSeccionVisitada(){
        this.seccionesVisitadas += this.seccionActive;
        
        this.subscription = this.service.seccionVisitada(this.auxiliarSeccionesVisitadas).subscribe(
            (result:any) => {
                if (result.status)
                    return result.status;
                else
                    throw new console.error("Ocurrio un error al actualizar la seccion.");
            },
        )
        
        this.isSeccionVisitada();
    }

    editarCuenta() {
        this.sessionDataService.editarCuenta();
        return false; // <- Prevent href del a
    }

    isAuthorized(permiso: string) {
        return this.securityService.tienePermiso(permiso);
    }

    isAmbos() {
        return this.granosFlag == "A" && this.isAuthorized('CONSULTAR HOME') && this.isAuthorized('CONSULTAR HOME NG');
    }

    isCliente(){
        return sessionStorage.getItem('tipoUsuario') == 'CLI'
    }

    setGranos() {
        this.granosSelected = "G";
        this.sessionDataService.setGranosSelected("G");
        sessionStorage.setItem("granosSelected", "G");
        this.goToSeccion('/home');
    }

    setNoGranos() {
        this.granosSelected = "N";
        this.sessionDataService.setGranosSelected("N");
        sessionStorage.setItem("granosSelected", "N");
        this.goToSeccion('/home-ngs');
    }

    isGranosSelected() {
        return this.granosSelected == "G";
    }

    isNoGranosSelected() {
        return this.granosSelected == "N";
    }

    noticiasNotEmpty() {
        return this.noticias != undefined && this.noticias.noticias != undefined;
    }

    openModalNoticia(noticia: any) {
        this.showLiquidaciones = false;
        this.showComprobantes = false;
        this.showDetalle = false;
        this.showFlete = false;
        this.showNoticias = true;
        this.modalHeader = "Noticias";
        this.modalNoticias = noticia;
        this.modal.open();
    }

    hideModal() {
        this.modal.close();
    }

    ngOnDestroy() {
        this.renderer.setElementClass(document.body, 'wrapper', false);
    }

    isSelectedMenu(menu: string) {
        return menu == this.menuActive;
    }

    isSelectedSeccion(seccion: string) {
        return seccion == this.seccionActive;
    }

    goToSeccion(path: string) {
        $("#mySidenav").css({ 'right': '-270px' });
        $("#myMenuClose").css({ 'display': 'none' });
        $("#myMenuOpen").css({ 'display': 'block' });
        $("#coverAll").fadeOut();
        this.navService.navegarSeccion(path);
        return false;
    }

    goToSeccionParamFromModal(path: string, param: string) {
        this.modal.close();
        this.navService.navegarSeccionParam(path, param);
        return false;
    }

    setErrorMsjModal(texto: string) {
        this.mensajeModalComponent.setErrorMsg(texto);
    }

    setSuccessMsjModal(texto: string) {
        this.mensajeModalComponent.setSuccessMsg(texto);
    }

    setEmptyMsjModal() {
        this.mensajeModalComponent.setMsgsEmpty();
    }

    setMsjError(texto: string) {
        this.mensajeError = texto;
        this.mensajeInfo = "";
        this.mensajeSuccess = "";
    }

    setMsjInfo(texto: string) {
        this.mensajeInfo = texto;
        this.mensajeError = "";
        this.mensajeSuccess = "";
    }

    setMsjSuccess(texto: string) {
        this.mensajeError = "";
        this.mensajeInfo = "";
        this.mensajeSuccess = texto;
    }

    setMsjErrorModal(texto: string) {
        this.mensajeErrorModal = texto;
    }

    exportExcel(contrato: string, secuencia: string, comprobante: string) {
        this.setMsjErrorModal("");
        this.unsubscribe();
        this.subscription = this.service.downloadVinculacion(contrato, secuencia).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.setMsjErrorModal(result.error);
                } else if (result.info != undefined) {
                    this.setMsjErrorModal(result.info);
                } else {
                    var blob = new Blob([result], { type: 'application/octet-stream' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        //IE11
                        window.navigator.msSaveOrOpenBlob(blob, "ReporteVinculacion(" + comprobante + ").xls");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = "ReporteVinculacion(" + comprobante + ").xls";
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    }
                }
            },
            error => {
                this.setMsjErrorModal(error.message);
            }

        );
        return false;
    }

    exportExcelFleteProcedencia(contrato: string) {
        this.setMsjErrorModal("");
        this.unsubscribe();
        this.subscription = this.service.downloadProcedencia(contrato).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.setMsjErrorModal(result.error);
                } else if (result.info != undefined) {
                    this.setMsjErrorModal(result.info);
                } else {
                    var blob = new Blob([result], { type: 'application/octet-stream' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        //IE11
                        window.navigator.msSaveOrOpenBlob(blob, "ReporteFleteProcedencia(" + contrato + ").xls");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = "ReporteFleteProcedencia(" + contrato + ").xls";
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    }
                }
            },
            error => {
                this.setMsjErrorModal(error.message);
            }

        );
        return false;
    }

    guardarTarifas(fechaEmision: any) {
        this.setEmptyMsjModal();
        let fecha = "";
        if (fechaEmision) {
            let fechaEmisionArray = fechaEmision.split(" ");
            if (fechaEmisionArray.length > 0) {
                fecha = fechaEmisionArray[0];
            }
        }
        this.modalInfoFlete.fechaEmision = fecha;
        this.modalService.setDatosGuardarFlete(this.modalInfoFlete);
        return false;
    }

    cargarPDF(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.modalInfoFlete.pdf = fileList[0];
        }
    }

    cantidadNot() {
        if (this.noticias) {
            return this.noticias.cantidad;
        } else {
            return 0;
        }
    }

    isCorr() {
        return this.tipoUsuario.toUpperCase() == "CORR" || this.tipoUsuario.toUpperCase() == "NUECORR";
    }

    public unsubscribe() {
        if (this.subscription != undefined)
            this.subscription.unsubscribe();
    }

    goToDataAgro() {
        this.setMsjErrorModal("");
        this.unsubscribe();
        this.subscription = this.service.goToDataAgro().subscribe(
            (result:any) => {
                this.setMsjErrorModal("");
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.data == undefined) {
                    alert("Ocurrio un error inesperado al intentar obtener la URL de destino");
                } else if (result.data.error != undefined && result.data.error != "") {
                    alert(result.error);
                } else if (result.data.url == undefined || result.data.url == "") {
                    alert("No se pudo obtener la URL de destino");
                } else {
                    window.open(result.data.url, "_blank");
                }
            },
            error => {
                this.setMsjErrorModal(error.message);
                return false;
            }
        );
        return false;
    }

    clickLogo() {
        if (this.isAuthorized('CONSULTAR HOME') && this.isGranosSelected()) {
            this.goToSeccion('/home')
        }
        if (this.isAuthorized('CONSULTAR HOME NG') && this.isNoGranosSelected()) {
            this.goToSeccion('/home-ngs')
        }
    }

    validarSreen(){
        var size = window.innerWidth;

        if (size <= 950) {
            this.menuSmall = true;
        }
        else{
            this.menuSmall = false;
        }
    }

    updateQuantity(quantity: number) {
        this.quantityCommunications = quantity;
      }

    ngOnInit(){
        this.validarSreen();
    }

    cerrarComunicaciones(): void{
        $("#notificationClose, #notificationSmall").css({ "display": "none" });
        $("#notificationOpen").css({ "display": "block" });
        $("#notificationSmall").css({ "right" : "-270px" });
        $("#coverAll").fadeOut();
    }
}