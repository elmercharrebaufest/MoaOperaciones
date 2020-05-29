"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var layout_service_1 = require("./layout.service");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var NavService_1 = require("./../common/services/NavService");
var ModalService_1 = require("./../common/services/ModalService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var SecurityService_1 = require("./../common/services/SecurityService");
var mensaje_modal_component_1 = require("./../common/view-child/mensaje-modal/mensaje-modal.component");
var login_guard_1 = require("./../common/security/login-guard");
var LayoutComponent = /** @class */ (function () {
    function LayoutComponent(service, sessionDataService, navService, loginGuard, router, renderer, modalService, securityService, floatMsgService) {
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
        this.noticiasCantidad = 0;
        this.showLiquidaciones = false;
        this.showComprobantes = false;
        this.showDetalle = false;
        this.showFlete = false;
        this.showNoticias = false;
        this.showFleteProcedencia = false;
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
        this.mensajeModalComponent = new mensaje_modal_component_1.MensajeModalComponent();
    };
    LayoutComponent.prototype.cerrarSesion = function () {
        this.sessionDataService.logout();
        return false; // <- Prevent href del a
    };
    LayoutComponent.prototype.isAuthorized = function (permiso) {
        return this.securityService.tienePermiso(permiso);
    };
    LayoutComponent.prototype.isAmbos = function () {
        return this.granosFlag == "A";
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
        return this.tipoUsuario == "CORR";
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
    __decorate([
        core_1.ViewChild("myModal"),
        __metadata("design:type", Object)
    ], LayoutComponent.prototype, "modal", void 0);
    __decorate([
        core_1.ViewChild("mensajeModal"),
        __metadata("design:type", mensaje_modal_component_1.MensajeModalComponent)
    ], LayoutComponent.prototype, "mensajeModalComponent", void 0);
    LayoutComponent = __decorate([
        core_1.Component({
            selector: 'layout-app',
            templateUrl: "./app/layout/layout.component.html?v=" + new Date().getTime(),
            providers: [layout_service_1.LayoutService]
        }),
        __metadata("design:paramtypes", [layout_service_1.LayoutService, SessionDataService_1.SessionDataService, NavService_1.NavService, login_guard_1.LoginGuard, router_1.Router, core_1.Renderer, ModalService_1.ModalService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService])
    ], LayoutComponent);
    return LayoutComponent;
}());
exports.LayoutComponent = LayoutComponent;
//# sourceMappingURL=layout.component.js.map