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
import { Component } from '@angular/core';
import { LiquidacionService } from './liquidacion.service';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/Seccion';
import { ModalService } from './../common/services/ModalService';
var LiquidacionBaseComponent = /** @class */ (function (_super) {
    __extends(LiquidacionBaseComponent, _super);
    function LiquidacionBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroProducto = null;
        _this.productoSelected = "";
        _this.filtroObservacion = null;
        _this.filtroVendedor = null;
        _this.observacionSelected = "";
        _this.filtroComprobanteOContrato = "";
        _this.tituloArchivoModal = "";
        _this.tituloArchivoPDF = "Documento";
        return _this;
    }
    LiquidacionBaseComponent.prototype.checkPermisos = function () {
        this.securityService.tienePermisoRedirect("CONSULTAR LIQUIDACIONES");
    };
    LiquidacionBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/liquidacion/aprobada', 'liquidacion', 'Aprobadas'), new Seccion('/liquidacion/observada', 'liquidacion', 'Observadas'), new Seccion('/liquidacion/paga', 'liquidacion', 'Pagas')]);
        this.getData();
    };
    LiquidacionBaseComponent.prototype.setFiltroProducto = function (producto) {
        this.productoSelected = producto;
    };
    LiquidacionBaseComponent.prototype.setFiltroVendedor = function (vendedor) {
        this.observacionSelected = vendedor;
    };
    LiquidacionBaseComponent.prototype.isVisible = function () {
        if (this.data && this.data.liquidaciones && this.data.liquidaciones.length != 0)
            return true;
        else
            return false;
    };
    LiquidacionBaseComponent.prototype.showModal = function (contrato, secuencia, comprobante) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getVinculacion(contrato, secuencia).subscribe(function (result) {
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
                //this.data = result.data;
                _this.modalService.openModalLiquidacion("Cartas de Portes - Comp.: " + comprobante, result.data.data, contrato, secuencia, comprobante);
            }
        }, function (error) {
            // this.spinnerComponent.hideIt();
            _this.floatMsgService.setErrorMsg(error.message);
        });
        return false;
    };
    LiquidacionBaseComponent.prototype.descargaPDF = function (documento, ejercicio) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(function (result) {
            _this.spinnerComponent.hideIt();
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
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivoPDF + documento + ".pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivoPDF + documento + ".pdf";
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
    LiquidacionBaseComponent.prototype.vaciarFiltros = function () {
        this.filtroProducto = null;
        this.filtroObservacion = null;
        this.filtroComprobanteOContrato = "";
        this.productoSelected = "";
        this.observacionSelected = "";
    };
    LiquidacionBaseComponent.prototype.cargarFiltrosVariables = function (result) {
        if (result.filtroProducto != undefined)
            this.filtroProducto = result.filtroProducto.options;
        if (result.filtroVendedor != undefined)
            this.filtroObservacion = result.filtroObservacion.options;
    };
    LiquidacionBaseComponent = __decorate([
        Component({
            selector: 'app-liquidacion',
            template: "",
            providers: [LiquidacionService]
        }),
        __metadata("design:paramtypes", [LiquidacionService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], LiquidacionBaseComponent);
    return LiquidacionBaseComponent;
}(ListBaseComponent));
export { LiquidacionBaseComponent };
//# sourceMappingURL=liquidacion.component.js.map