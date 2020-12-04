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
import { PagoService } from './pago.service';
import { SessionDataService } from './../common/services/SessionDataService';
import { ModalService } from './../common/services/ModalService';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
var PagoComponent = /** @class */ (function (_super) {
    __extends(PagoComponent, _super);
    function PagoComponent(service, navService, sessionDateService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDateService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDateService = sessionDateService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroID = "";
        _this.filtroNroPago = "";
        return _this;
    }
    PagoComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.getData();
    };
    PagoComponent.prototype.isVisible = function () {
        if (this.data && this.data.pagos.length != 0)
            return true;
        else
            return false;
    };
    PagoComponent.prototype.showModalComprobantes = function (documento, fecha, fiscYear) {
        var _this = this;
        this.unsubscribe();
        this.floatMsgService.setMsgsEmpty();
        this.subscription = this.service.getComprobantes(documento, fecha, fiscYear).subscribe(function (result) {
            //this.spinnerComponent.hideIt();
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
                _this.modalService.openModalComprobante("Comprobantes de Pago", result.comprobantes);
            }
        }, function (error) {
            // this.spinnerComponent.hideIt();
            _this.floatMsgService.setErrorMsg(error.message);
        });
        return false;
    };
    PagoComponent.prototype.descargaPDF = function (documento, ejercicio) {
        var _this = this;
        var tituloArchivoPDF = "Documento-";
        this.unsubscribe();
        this.mensajeComponent.setMsgsEmpty();
        this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(function (result) {
            //this.spinnerComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                //this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                //this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, tituloArchivoPDF + documento + ".pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = tituloArchivoPDF + documento + ".pdf";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            //this.spinnerComponent.hideIt();
            //this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    PagoComponent.prototype.vaciarFiltros = function () {
        this.filtroID = "";
        this.filtroNroPago = "";
    };
    PagoComponent.prototype.isNegative = function (valor) {
        return valor < 0;
    };
    PagoComponent = __decorate([
        Component({
            selector: 'app-pago',
            template: "",
            providers: [PagoService]
        }),
        __metadata("design:paramtypes", [PagoService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], PagoComponent);
    return PagoComponent;
}(ListBaseComponent));
export { PagoComponent };
//# sourceMappingURL=pago.component.js.map