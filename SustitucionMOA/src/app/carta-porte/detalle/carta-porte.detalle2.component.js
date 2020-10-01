var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, ViewChild } from '@angular/core';
import { CartaPorteService } from './../carta-porte2.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { Router, ActivatedRoute } from '@angular/router';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { Seccion } from './../../common/models/seccion';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
var CartaPorteDetalleComponent = /** @class */ (function (_super) {
    __extends(CartaPorteDetalleComponent, _super);
    function CartaPorteDetalleComponent(service, navService, route, router, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.route = route;
        _this.router = router;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.cartaPorteId = "";
        _this.tituloArchivo = "ReporteCartaPorteDetalle.xls";
        _this.fotoSrc = "";
        _this.showModalBox = false;
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    CartaPorteDetalleComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("carta-porte", "Detalle");
    };
    CartaPorteDetalleComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CARTAS PORTE DETALLE");
        var secciones = [new Seccion('/carta-porte/descarga', 'carta-porte', 'Descargas'), new Seccion('/carta-porte/aplicacion', 'carta-porte', 'Aplicaciones')];
        if (this.isAuthorized("CREAR FORMULARIO CCPP"))
            secciones.push(new Seccion('/carta-porte/formulario', 'carta-porte', 'Formulario'));
        this.navService.setSeccionList(secciones);
        this.getData();
    };
    CartaPorteDetalleComponent.prototype.ngAfterViewInit = function () {
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    };
    CartaPorteDetalleComponent.prototype.getData = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach(function (params) {
            _this.cartaPorteId = params['id'];
            _this.unsubscribe();
            _this.subscription = _this.service.getDetalle(_this.cartaPorteId).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.data = result.data;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        });
    };
    CartaPorteDetalleComponent.prototype.exportExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalle(this.cartaPorteId).subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivo);
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivo;
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false; // <- Prevent href del a
    };
    CartaPorteDetalleComponent.prototype.descargarPDF = function () {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.exportPDFCalidad(this.cartaPorteId).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.floatMsgService.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.floatMsgService.setInfoMsg(result.info);
            }
            else {
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, "Calidad Carta de Porte(" + _this.cartaPorteId + ").pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "Calidad Carta de Porte(" + _this.cartaPorteId + ").pdf";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.floatMsgService.setErrorMsg(error.message);
        });
        return false;
    };
    CartaPorteDetalleComponent.prototype.isExportVisible = function () {
        this.spinnerSmallComponent.visible;
    };
    CartaPorteDetalleComponent.prototype.showDataPlus = function (calidad) {
        return this.tieneData(calidad.kgNetos) || this.tieneData(calidad.kgDescuento) || this.tieneData(calidad.kgAplicados) || this.tieneData(calidad.porcentajeDescuento);
    };
    CartaPorteDetalleComponent.prototype.tieneData = function (value) {
        return value != undefined && value != 0 && value != "";
    };
    CartaPorteDetalleComponent.prototype.abrirModal = function () {
        var _this = this;
        console.log("funcionHOLA");
        this.spinnerSmallComponent.showIt();
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getFotos(this.cartaPorteId).subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.floatMsgService.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.floatMsgService.setInfoMsg(result.info);
            }
            else {
                _this.fotoSrc = 'data:image/png;base64,' + result[0].Foto;
                document.getElementById("openModalHiddenButton").click();
                return true;
            }
        }, function (error) {
            _this.floatMsgService.setErrorMsg(error.message);
        });
        return false;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], CartaPorteDetalleComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], CartaPorteDetalleComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild("smallSpinner"),
        __metadata("design:type", SpinnerSmallComponent)
    ], CartaPorteDetalleComponent.prototype, "spinnerSmallComponent", void 0);
    CartaPorteDetalleComponent = __decorate([
        Component({
            selector: 'app-carta-porte-detalle',
            templateUrl: "carta-porte.detalle.component.html",
            providers: [CartaPorteService]
        }),
        __metadata("design:paramtypes", [CartaPorteService, NavService, ActivatedRoute, Router, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], CartaPorteDetalleComponent);
    return CartaPorteDetalleComponent;
}(BaseComponent));
export { CartaPorteDetalleComponent };
//# sourceMappingURL=carta-porte.detalle2.component.js.map