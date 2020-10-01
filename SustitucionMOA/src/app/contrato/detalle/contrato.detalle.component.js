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
import { Router, ActivatedRoute } from '@angular/router';
import { ContratoService, ContratoFijacionService } from './../contrato.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { Seccion } from './../../common/models/Seccion';
import { ModalService } from './../../common/services/ModalService';
var ContratoDetalleComponent = /** @class */ (function (_super) {
    __extends(ContratoDetalleComponent, _super);
    function ContratoDetalleComponent(route, router, service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.route = route;
        _this.router = router;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.data = null;
        _this.tituloArchivoPDF = "BoletoFisico";
        _this.tituloArchivoExcel = "ReporteContratoDetalle.xls";
        _this.numeroContratoId = "";
        _this.filtroCCPP = "";
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    ContratoDetalleComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Detalle");
    };
    ContratoDetalleComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CONTRATO DETALLE");
        this.navService.setSeccionList([new Seccion('/contrato/vigente', 'contrato', 'Vigentes'), new Seccion('/contrato/fijacion', 'contrato', 'Fijaciones'), new Seccion('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new Seccion('/contrato/anulacion', 'contrato', 'Anulaciones')]);
        this.getData();
    };
    ContratoDetalleComponent.prototype.ngAfterViewInit = function () {
        this.spinnerSmallExportComponent = new SpinnerSmallComponent();
        this.spinnerSmallPDFComponent = new SpinnerSmallComponent();
    };
    ContratoDetalleComponent.prototype.getData = function () {
        var _this = this;
        this.data = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.subscribe(function (params) {
            _this.numeroContratoId = params['id'];
            _this.unsubscribe();
            _this.subscription = _this.service.getDetalle(_this.numeroContratoId).subscribe(function (result) {
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
                    _this.data = result;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
            //this.navService.setSeccionActive('Fijaciones');
        });
    };
    ContratoDetalleComponent.prototype.downloadBoletoFisico = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallPDFComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.downloadBoletoFisico(this.numeroContratoId).subscribe(function (result) {
            _this.spinnerSmallPDFComponent.hideIt();
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
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivoPDF + _this.numeroContratoId + ".pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivoPDF + _this.numeroContratoId + ".pdf";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallPDFComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ContratoDetalleComponent.prototype.exportarExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallExportComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalle(this.numeroContratoId).subscribe(function (result) {
            _this.spinnerSmallExportComponent.hideIt();
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
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivoExcel);
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivoExcel;
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallExportComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ContratoDetalleComponent.prototype.descargarPDF = function () {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.exportPDFCalidad(this.numeroContratoId).subscribe(function (result) {
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
                    window.navigator.msSaveOrOpenBlob(blob, "Calidad Contrato(" + _this.numeroContratoId + ").pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "Calidad Contrato(" + _this.numeroContratoId + ").pdf";
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
    ContratoDetalleComponent.prototype.showDataPlus = function (registro) {
        return this.tieneData(registro.kgNetos) || this.tieneData(registro.kgDto) || this.tieneData(registro.kgApli) || this.tieneData(registro.dto);
    };
    ContratoDetalleComponent.prototype.tieneData = function (value) {
        return value != undefined && value != 0 && value != "" && value != "0 KG" && value != "0%";
    };
    ContratoDetalleComponent.prototype.descargaRetencionesPDF = function (pago) {
        // Descarga PDF TODO
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableBoletosResponsive = function (Boleto) {
        this.modalService.openModalTableResponsive("Oblea Boleto", [
            { etiqueta: "Envío bolsa", valor: Boleto.envioBolsa },
            { etiqueta: "Vuelta Bolsa", valor: Boleto.vueltaBolsa },
            { etiqueta: "Oblea Bolsa", valor: Boleto.obleaBolsa },
            { etiqueta: "Envío Afip", valor: Boleto.envioAfip },
            { etiqueta: "Vuelta Afip", valor: Boleto.vueltaAfip },
            { etiqueta: "REG. Afip", valor: Boleto.obleaAfip },
            { etiqueta: "Oblea Prov. Plan Canje", valor: Boleto.provPlanCanje },
            { etiqueta: "Envío Sellado", valor: Boleto.envioSellado },
            { etiqueta: "Vuelta Sellado", valor: Boleto.vueltaSellado }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTablePagosResponsive = function (Pago) {
        this.modalService.openModalTableResponsive("Pago", [
            { etiqueta: "Fecha", valor: Pago.fecha },
            { etiqueta: "ID pago", valor: Pago.idPago },
            { etiqueta: "Brutor", valor: Pago.brutoString },
            { etiqueta: "Iva", valor: Pago.ivaString },
            { etiqueta: "Retenciones", valor: Pago.retencionesString },
            { etiqueta: "Total", valor: Pago.netoString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableLiquidacionesResponsive = function (Liquidacion) {
        this.modalService.openModalTableResponsive("Liquidación", [
            { etiqueta: "Fecha", valor: Liquidacion.fecha },
            { etiqueta: "Tipo", valor: Liquidacion.tipo },
            { etiqueta: "Comprobante", valor: Liquidacion.comprobante },
            { etiqueta: "KG", valor: Liquidacion.cantidadString },
            { etiqueta: "Precio/tn", valor: Liquidacion.precioString },
            { etiqueta: "Total", valor: Liquidacion.totalString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableLFijacionesResponsive = function (Fijacion) {
        this.modalService.openModalTableResponsive("Fijación", [
            { etiqueta: "Fecha", valor: Fijacion.fecha },
            { etiqueta: "Nº Fijación", valor: Fijacion.nroFija },
            { etiqueta: "Kilos Fijados", valor: Fijacion.kilosFijaString },
            { etiqueta: "Precio/TN", valor: Fijacion.precioString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableAmpliacionesAnulacionesResponsive = function (AmpAnul) {
        this.modalService.openModalTableResponsive("Ampliación / Anulación", [
            { etiqueta: "Tipo", valor: AmpAnul.tipo },
            { etiqueta: "Fecha", valor: AmpAnul.fecha },
            { etiqueta: "Cantidad", valor: AmpAnul.cantidadString },
            { etiqueta: "Precio/Multa", valor: AmpAnul.importeString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableAplicacionesResponsive = function (Aplicacion) {
        this.modalService.openModalTableResponsive("Aplicación", [
            { etiqueta: "Fecha", valor: Aplicacion.fecha },
            { etiqueta: "Carta de Porte", valor: Aplicacion.ccpp },
            { etiqueta: "Lugar de Descarga", valor: Aplicacion.descarga },
            { etiqueta: "KG Brutos", valor: Aplicacion.kgBrutosString },
            { etiqueta: "KG Aplicados", valor: Aplicacion.kgNetosString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.showModalTableHijosResponsive = function (Hijo) {
        this.modalService.openModalTableResponsive("Hijo", [
            { etiqueta: "Fecha de Concertación", valor: Hijo.fecha },
            { etiqueta: "Contrato Molinos", valor: Hijo.contrMolinos },
            { etiqueta: "Contrato Proveedor", valor: Hijo.contrProve },
            { etiqueta: "Pactado", valor: Hijo.cantidadString },
            { etiqueta: "Precio/tn", valor: Hijo.precioString }
        ]);
        return false;
    };
    ContratoDetalleComponent.prototype.isData = function () {
        return this.data != null;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], ContratoDetalleComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], ContratoDetalleComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild("spinnerSmallExport"),
        __metadata("design:type", SpinnerSmallComponent)
    ], ContratoDetalleComponent.prototype, "spinnerSmallExportComponent", void 0);
    __decorate([
        ViewChild("spinnerSmallPDF"),
        __metadata("design:type", SpinnerSmallComponent)
    ], ContratoDetalleComponent.prototype, "spinnerSmallPDFComponent", void 0);
    ContratoDetalleComponent = __decorate([
        Component({
            selector: 'app-contrato-detalle',
            templateUrl: "contrato.detalle.component.html",
            providers: [{ provide: ContratoService, useClass: ContratoFijacionService }]
        }),
        __metadata("design:paramtypes", [ActivatedRoute,
            Router,
            ContratoService,
            NavService,
            SecurityService,
            SessionDataService,
            FloatMsgService,
            ModalService])
    ], ContratoDetalleComponent);
    return ContratoDetalleComponent;
}(BaseComponent));
export { ContratoDetalleComponent };
//# sourceMappingURL=contrato.detalle.component.js.map