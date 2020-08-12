import { Component, Renderer, OnDestroy, ViewChild } from '@angular/core';
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
declare var $: any;

@Component({
    selector: 'app-layout',
    templateUrl: `./app/layout/layout.component.html?v=${new Date().getTime()}`,
    providers: [ LayoutService ]
})

export class LayoutComponent implements OnDestroy {

    titulo: string;
    username: string;
    nombre: string;
    proveedor: string;
    permisos: Array<string>;
    granosFlag: string;
    granosSelected: string;
    tipoUsuario: string;
    noticias: any;
    noticiasCantidad = 0;
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
    seccionActive: string;
    menuActive: string = 'home';
    mensajeError = '';
    mensajeInfo = '';
    mensajeSuccess = '';
    mensajeErrorModal = '';
    subscription: any;

    @ViewChild("myModal") modal: any;

    @ViewChild("mensajeModal")
    protected mensajeModalComponent: MensajeModalComponent;

    constructor(private service: LayoutService, private sessionDataService: SessionDataService, private navService: NavService, private loginGuard: LoginGuard, private router: Router, protected renderer: Renderer, private modalService: ModalService, private securityService: SecurityService, private floatMsgService: FloatMsgService) {

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

        navService.seccionList$.subscribe(
            seccionList => {
                this.seccionList = seccionList
            });
        navService.seccionActive$.subscribe(
            seccionActive => {
                this.seccionActive = seccionActive
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
                this.setEmptyMsjModal();
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
     
    }

    ngAfterViewInit() {
        this.modalService.modal = this.modal;
        this.mensajeModalComponent = new MensajeModalComponent();
    }

    cerrarSesion() {
        this.sessionDataService.logout();
        return false; // <- Prevent href del a
    }


    editarCuenta() {
        this.sessionDataService.editarCuenta();
        return false; // <- Prevent href del a
    }

    isAuthorized(permiso: string) {
        return this.securityService.tienePermiso(permiso);
    }

    isAmbos() {
        return this.granosFlag == "A";
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
            result => {
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
            result => {
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
        return this.tipoUsuario == "CORR";
    }

    public unsubscribe() {
        if (this.subscription != undefined)
            this.subscription.unsubscribe();
    }

    goToDataAgro() {
        this.setMsjErrorModal("");
        this.unsubscribe();
        this.subscription = this.service.goToDataAgro().subscribe(
            result => {
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
}