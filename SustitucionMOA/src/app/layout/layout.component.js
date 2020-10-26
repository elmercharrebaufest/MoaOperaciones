var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, Renderer, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router } from "@angular/router";
import { LayoutService } from './layout.service';
import { SessionDataService } from './../common/services/SessionDataService';
import { NavService } from './../common/services/NavService';
import { ModalService } from './../common/services/ModalService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { MensajeModalComponent } from './../common/view-child/mensaje-modal/mensaje-modal.component';
import { LoginGuard } from './../common/security/login-guard';
var LayoutComponent = /** @class */ (function () {
    function LayoutComponent(service, sessionDataService, navService, loginGuard, router, renderer, modalService, securityService, floatMsgService, cd) {
        var _this = this;
        this.service = service;
        this.sessionDataService = sessionDataService;
        this.navService = navService;
        this.loginGuard = loginGuard;
        this.router = router;
        this.renderer = renderer;
        this.modalService = modalService;
        this.securityService = securityService;
        this.floatMsgService = floatMsgService;
        this.cd = cd;
        this.noticiasCantidad = 0;
        this.showLiquidaciones = false;
        this.showComprobantes = false;
        this.showDetalle = false;
        this.showFlete = false;
        this.showNoticias = false;
        this.showFleteProcedencia = false;
        this.tieneSecciones = false;
        this.menuActive = 'home';
        this.mensajeError = '';
        this.mensajeInfo = '';
        this.mensajeSuccess = '';
        this.mensajeErrorModal = '';
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
        sessionDataService.username$.subscribe(function (username) {
            _this.username = username;
        });
        sessionDataService.nombre$.subscribe(function (nombre) {
            _this.nombre = nombre;
        });
        sessionDataService.proveedor$.subscribe(function (proveedor) {
            _this.proveedor = proveedor;
        });
        sessionDataService.granosFlag$.subscribe(function (granosFlag) {
            _this.granosFlag = granosFlag;
        });
        sessionDataService.granosSelected$.subscribe(function (granosSelected) {
            _this.granosSelected = granosSelected;
        });
        sessionDataService.tipoUsuario$.subscribe(function (tipoUsuario) {
            _this.tipoUsuario = tipoUsuario;
        });
        sessionDataService.noticias$.subscribe(function (noticias) {
            _this.noticias = noticias;
        });
        navService.seccionList$.subscribe(function (seccionList) {
            _this.seccionList = seccionList;
            _this.tieneSecciones = seccionList.length > 0;
            _this.cd.detectChanges();
        });
        navService.seccionActive$.subscribe(function (seccionActive) {
            _this.seccionActive = seccionActive;
        });
        navService.menuActive$.subscribe(function (menuActive) {
            _this.menuActive = menuActive;
        });
        modalService.modalHeader$.subscribe(function (modalHeader) {
            _this.modalHeader = modalHeader;
        });
        modalService.modalInfoLiquidacion.subscribe(function (modalInfoLiquidacion) {
            _this.modalInfoLiquidacion = modalInfoLiquidacion;
        });
        modalService.modalLiquidacionContrato.subscribe(function (modalLiquidacionContrato) {
            _this.modalLiquidacionContrato = modalLiquidacionContrato;
        });
        modalService.modalLiquidacionSecuencia.subscribe(function (modalLiquidacionSecuencia) {
            _this.modalLiquidacionSecuencia = modalLiquidacionSecuencia;
        });
        modalService.modalLiquidacionComprobante.subscribe(function (modalLiquidacionComprobante) {
            _this.modalLiquidacionComprobante = modalLiquidacionComprobante;
        });
        modalService.modalShowLiquidacion.subscribe(function (modalShowLiquidacion) {
            _this.showLiquidaciones = modalShowLiquidacion;
        });
        modalService.modalInfoComprobante.subscribe(function (modalInfoComprobante) {
            _this.modalInfoComprobante = modalInfoComprobante;
        });
        modalService.modalShowComprobante.subscribe(function (modalShowComprobante) {
            _this.showComprobantes = modalShowComprobante;
        });
        modalService.modalShowNoticia.subscribe(function (modalShowNoticia) {
            _this.showNoticias = modalShowNoticia;
        });
        modalService.modalInfoDetalle.subscribe(function (modalInfoDetalle) {
            _this.modalInfoDetalle = modalInfoDetalle;
        });
        modalService.modalShowDetalle.subscribe(function (modalShowDetalle) {
            _this.showDetalle = modalShowDetalle;
        });
        modalService.modalInfoFlete.subscribe(function (modalInfoFlete) {
            _this.modalInfoFlete = modalInfoFlete;
        });
        modalService.modalShowFlete.subscribe(function (modalShowFlete) {
            _this.setEmptyMsjModal();
            _this.showFlete = modalShowFlete;
        });
        modalService.modalContratoFleteProcedencia.subscribe(function (modalContratoFleteProcedencia) {
            _this.modalInfoContratoProcedencia = modalContratoFleteProcedencia;
        });
        modalService.modalInfoFleteProcedencia.subscribe(function (modalInfoFleteProcedencia) {
            _this.modalInfoFleteProcedencia = modalInfoFleteProcedencia;
        });
        modalService.modalShowFleteProcedencia.subscribe(function (modalShowFleteProcedencia) {
            _this.setEmptyMsjModal();
            _this.showFleteProcedencia = modalShowFleteProcedencia;
        });
        modalService.modalFooter$.subscribe(function (modalFooter) {
            _this.modalFooter = modalFooter;
        });
        modalService.modalMsjError.subscribe(function (modalMsjError) {
            _this.setErrorMsjModal(modalMsjError);
        });
        modalService.modalMsjSuccess.subscribe(function (modalMsjSuccess) {
            _this.setSuccessMsjModal(modalMsjSuccess);
        });
        floatMsgService.errorMsj$.subscribe(function (errorMsj) {
            _this.setMsjError(errorMsj);
        });
        floatMsgService.infoMsj$.subscribe(function (infoMsj) {
            _this.setMsjInfo(infoMsj);
        });
        floatMsgService.successMsj$.subscribe(function (successMsj) {
            _this.setMsjSuccess(successMsj);
        });
    }
    LayoutComponent.prototype.ngAfterViewInit = function () {
        this.modalService.modal = this.modal;
        this.mensajeModalComponent = new MensajeModalComponent();
    };
    LayoutComponent.prototype.cerrarSesion = function () {
        this.sessionDataService.logout();
        return false; // <- Prevent href del a
    };
    LayoutComponent.prototype.editarCuenta = function () {
        this.sessionDataService.editarCuenta();
        return false; // <- Prevent href del a
    };
    LayoutComponent.prototype.isAuthorized = function (permiso) {
        return this.securityService.tienePermiso(permiso);
    };
    LayoutComponent.prototype.isAmbos = function () {
        return this.isAuthorized('CONSULTAR HOME') && this.isAuthorized('CONSULTAR HOME NG');
    };
    LayoutComponent.prototype.setGranos = function () {
        this.granosSelected = "G";
        this.sessionDataService.setGranosSelected("G");
        sessionStorage.setItem("granosSelected", "G");
        this.goToSeccion('/home');
    };
    LayoutComponent.prototype.setNoGranos = function () {
        this.granosSelected = "N";
        this.sessionDataService.setGranosSelected("N");
        sessionStorage.setItem("granosSelected", "N");
        this.goToSeccion('/home-ngs');
    };
    LayoutComponent.prototype.isGranosSelected = function () {
        return this.granosSelected == "G";
    };
    LayoutComponent.prototype.isNoGranosSelected = function () {
        return this.granosSelected == "N";
    };
    LayoutComponent.prototype.noticiasNotEmpty = function () {
        return this.noticias != undefined && this.noticias.noticias != undefined;
    };
    LayoutComponent.prototype.openModalNoticia = function (noticia) {
        this.showLiquidaciones = false;
        this.showComprobantes = false;
        this.showDetalle = false;
        this.showFlete = false;
        this.showNoticias = true;
        this.modalHeader = "Noticias";
        this.modalNoticias = noticia;
        this.modal.open();
    };
    LayoutComponent.prototype.hideModal = function () {
        this.modal.close();
    };
    LayoutComponent.prototype.ngOnDestroy = function () {
        this.renderer.setElementClass(document.body, 'wrapper', false);
    };
    LayoutComponent.prototype.isSelectedMenu = function (menu) {
        return menu == this.menuActive;
    };
    LayoutComponent.prototype.isSelectedSeccion = function (seccion) {
        return seccion == this.seccionActive;
    };
    LayoutComponent.prototype.goToSeccion = function (path) {
        $("#mySidenav").css({ 'right': '-270px' });
        $("#myMenuClose").css({ 'display': 'none' });
        $("#myMenuOpen").css({ 'display': 'block' });
        $("#coverAll").fadeOut();
        this.navService.navegarSeccion(path);
        return false;
    };
    LayoutComponent.prototype.goToSeccionParamFromModal = function (path, param) {
        this.modal.close();
        this.navService.navegarSeccionParam(path, param);
        return false;
    };
    LayoutComponent.prototype.setErrorMsjModal = function (texto) {
        this.mensajeModalComponent.setErrorMsg(texto);
    };
    LayoutComponent.prototype.setSuccessMsjModal = function (texto) {
        this.mensajeModalComponent.setSuccessMsg(texto);
    };
    LayoutComponent.prototype.setEmptyMsjModal = function () {
        this.mensajeModalComponent.setMsgsEmpty();
    };
    LayoutComponent.prototype.setMsjError = function (texto) {
        this.mensajeError = texto;
        this.mensajeInfo = "";
        this.mensajeSuccess = "";
    };
    LayoutComponent.prototype.setMsjInfo = function (texto) {
        this.mensajeInfo = texto;
        this.mensajeError = "";
        this.mensajeSuccess = "";
    };
    LayoutComponent.prototype.setMsjSuccess = function (texto) {
        this.mensajeError = "";
        this.mensajeInfo = "";
        this.mensajeSuccess = texto;
    };
    LayoutComponent.prototype.setMsjErrorModal = function (texto) {
        this.mensajeErrorModal = texto;
    };
    LayoutComponent.prototype.exportExcel = function (contrato, secuencia, comprobante) {
        var _this = this;
        this.setMsjErrorModal("");
        this.unsubscribe();
        this.subscription = this.service.downloadVinculacion(contrato, secuencia).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.setMsjErrorModal(result.error);
            }
            else if (result.info != undefined) {
                _this.setMsjErrorModal(result.info);
            }
            else {
                var blob = new Blob([result], { type: 'application/octet-stream' });
                if (window.navigator.msSaveOrOpenBlob) {
                    //IE11
                    window.navigator.msSaveOrOpenBlob(blob, "ReporteVinculacion(" + comprobante + ").xls");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "ReporteVinculacion(" + comprobante + ").xls";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                }
            }
        }, function (error) {
            _this.setMsjErrorModal(error.message);
        });
        return false;
    };
    LayoutComponent.prototype.exportExcelFleteProcedencia = function (contrato) {
        var _this = this;
        this.setMsjErrorModal("");
        this.unsubscribe();
        this.subscription = this.service.downloadProcedencia(contrato).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.setMsjErrorModal(result.error);
            }
            else if (result.info != undefined) {
                _this.setMsjErrorModal(result.info);
            }
            else {
                var blob = new Blob([result], { type: 'application/octet-stream' });
                if (window.navigator.msSaveOrOpenBlob) {
                    //IE11
                    window.navigator.msSaveOrOpenBlob(blob, "ReporteFleteProcedencia(" + contrato + ").xls");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "ReporteFleteProcedencia(" + contrato + ").xls";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                }
            }
        }, function (error) {
            _this.setMsjErrorModal(error.message);
        });
        return false;
    };
    LayoutComponent.prototype.guardarTarifas = function (fechaEmision) {
        this.setEmptyMsjModal();
        var fecha = "";
        if (fechaEmision) {
            var fechaEmisionArray = fechaEmision.split(" ");
            if (fechaEmisionArray.length > 0) {
                fecha = fechaEmisionArray[0];
            }
        }
        this.modalInfoFlete.fechaEmision = fecha;
        this.modalService.setDatosGuardarFlete(this.modalInfoFlete);
        return false;
    };
    LayoutComponent.prototype.cargarPDF = function (event) {
        var fileList = event.target.files;
        if (fileList.length > 0) {
            this.modalInfoFlete.pdf = fileList[0];
        }
    };
    LayoutComponent.prototype.cantidadNot = function () {
        if (this.noticias) {
            return this.noticias.cantidad;
        }
        else {
            return 0;
        }
    };
    LayoutComponent.prototype.isCorr = function () {
        return this.tipoUsuario.toUpperCase() == "CORR" || this.tipoUsuario.toUpperCase() == "NUECORR";
    };
    LayoutComponent.prototype.unsubscribe = function () {
        if (this.subscription != undefined)
            this.subscription.unsubscribe();
    };
    LayoutComponent.prototype.goToDataAgro = function () {
        var _this = this;
        this.setMsjErrorModal("");
        this.unsubscribe();
        this.subscription = this.service.goToDataAgro().subscribe(function (result) {
            _this.setMsjErrorModal("");
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.data == undefined) {
                alert("Ocurrio un error inesperado al intentar obtener la URL de destino");
            }
            else if (result.data.error != undefined && result.data.error != "") {
                alert(result.error);
            }
            else if (result.data.url == undefined || result.data.url == "") {
                alert("No se pudo obtener la URL de destino");
            }
            else {
                window.open(result.data.url, "_blank");
            }
        }, function (error) {
            _this.setMsjErrorModal(error.message);
            return false;
        });
        return false;
    };
    LayoutComponent.prototype.clickLogo = function () {
        if (this.isAuthorized('CONSULTAR HOME') && this.isGranosSelected()) {
            this.goToSeccion('/home');
        }
        if (this.isAuthorized('CONSULTAR HOME NG') && this.isNoGranosSelected()) {
            this.goToSeccion('/home-ngs');
        }
    };
    __decorate([
        ViewChild("myModal"),
        __metadata("design:type", Object)
    ], LayoutComponent.prototype, "modal", void 0);
    __decorate([
        ViewChild("mensajeModal"),
        __metadata("design:type", MensajeModalComponent)
    ], LayoutComponent.prototype, "mensajeModalComponent", void 0);
    LayoutComponent = __decorate([
        Component({
            selector: 'app-layout',
            templateUrl: "layout.component.html",
            providers: [LayoutService]
        }),
        __metadata("design:paramtypes", [LayoutService, SessionDataService,
            NavService, LoginGuard, Router,
            Renderer, ModalService, SecurityService, FloatMsgService,
            ChangeDetectorRef])
    ], LayoutComponent);
    return LayoutComponent;
}());
export { LayoutComponent };
//# sourceMappingURL=layout.component.js.map