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
import { HomeService } from './home.service';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { BaseComponent } from './../common/base-components/base-component';
import { ModalService } from './../common/services/ModalService';
var HomeComponent = /** @class */ (function (_super) {
    __extends(HomeComponent, _super);
    function HomeComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.visible = false;
        _this.cuentasCorrientes = null;
        _this.itemsPerPage = "10";
        _this.tituloArchivoPDF = "Documento";
        _this.checkPermisos();
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        _this.filtroFechaComponent = new FiltroFechaComponent();
        return _this;
    }
    HomeComponent.prototype.checkPermisos = function () {
        if (this.securityService.esGranosRedirect()) {
            this.securityService.tienePermisoRedirect("CONSULTAR HOME");
        }
    };
    HomeComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("home", "");
        this.sessionDataService.setGranosSelected("G");
        sessionStorage.setItem("granosSelected", "G");
    };
    HomeComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.navService.setSeccionList([]);
        this.getData();
    };
    HomeComponent.prototype.getData = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.vaciarDatos();
        this.visible = false;
        this.spinnerComponent.showIt();
        this.mensajeCtaCte = undefined;
        this.unsubscribe();
        this.subscription = this.service.getHomeInfo(this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(function (result) {
            _this.spinnerComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.visible = false;
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.visible = false;
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                _this.visible = true;
                if (result.data != null) {
                    if (result.data.resumen != null) {
                        _this.setData(result.data.resumen);
                        if (result.data.msjCtaCte != null && result.data.msjCtaCte != "") {
                            _this.mensajeCtaCte = result.data.msjCtaCte;
                            _this.cuentasCorrientes = null;
                        }
                        else {
                            _this.cuentasCorrientes = result.data.cuentasCorrientes;
                        }
                    }
                }
            }
        }, function (error) {
            _this.visible = false;
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    HomeComponent.prototype.descargaPDF = function (documento, ejercicio) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(function (result) {
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
    HomeComponent.prototype.setData = function (resumen) {
        this.vigentes = this.getCantidad("Vigentes", resumen);
        this.fijaciones = this.getCantidad("Fijaciones", resumen);
        this.ampliaciones = this.getCantidad("Ampliaciones", resumen);
        this.anulaciones = this.getCantidad("Anulaciones", resumen);
        this.descargas = this.getCantidad("Descargas", resumen);
        this.aplicaciones = this.getCantidad("Aplicaciones", resumen);
        this.aprobadas = this.getCantidad("Aprobadas", resumen);
        this.observadas = this.getCantidad("Observadas", resumen);
        this.pagas = this.getCantidad("Pagas", resumen);
        this.emitidos = this.getCantidad("Emitidos", resumen);
        this.iva = this.getCantidad("IVA", resumen);
        this.ganancias = this.getCantidad("Ganancias", resumen);
        this.iibb = this.getCantidad("IIBB", resumen);
    };
    HomeComponent.prototype.getCantidad = function (tipo, datos) {
        var cantidad = "0";
        for (var i = 0; i < datos.length; i++) {
            if (datos[i].descripcion == tipo) {
                return datos[i].cantidad;
            }
        }
        return cantidad;
    };
    HomeComponent.prototype.vaciarDatos = function () {
        this.cuentasCorrientes = null;
        this.vigentes = null;
        this.fijaciones = null;
        this.ampliaciones = null;
        this.anulaciones = null;
        this.descargas = null;
        this.aplicaciones = null;
        this.aprobadas = null;
        this.observadas = null;
        this.emitidos = null;
        this.iva = null;
        this.ganancias = null;
        this.iibb = null;
    };
    HomeComponent.prototype.isVisible = function () {
        return this.visible;
    };
    HomeComponent.prototype.isVisibleCtaCte = function () {
        return this.cuentasCorrientes != null && this.cuentasCorrientes.length > 0;
    };
    HomeComponent.prototype.showModalTableResponsive = function (cuentaCorriente) {
        this.modalService.openModalTableResponsive("Movimientos", [
            { etiqueta: "F. Emisión", valor: cuentaCorriente.docDate },
            { etiqueta: "F. Vto.", valor: cuentaCorriente.fecVto },
            { etiqueta: "Nº Cte.", valor: cuentaCorriente.docNo },
            { etiqueta: "Descripción", valor: cuentaCorriente.descripcion },
            { etiqueta: "Contrato", valor: cuentaCorriente.contrato },
            { etiqueta: "Moneda", valor: cuentaCorriente.moneda },
            { etiqueta: "TC", valor: cuentaCorriente.ukursString },
            { etiqueta: "Debe", valor: cuentaCorriente.debeString },
            { etiqueta: "Haber", valor: cuentaCorriente.haberString }
        ]);
        return false;
    };
    __decorate([
        ViewChild(FiltroFechaComponent),
        __metadata("design:type", FiltroFechaComponent)
    ], HomeComponent.prototype, "filtroFechaComponent", void 0);
    __decorate([
        ViewChild("msjHome"),
        __metadata("design:type", MensajeComponent)
    ], HomeComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], HomeComponent.prototype, "spinnerComponent", void 0);
    HomeComponent = __decorate([
        Component({
            selector: 'app-home',
            //template: '<h1>{{titulo}}</h1>'
            templateUrl: "home.component.html",
            providers: [HomeService]
        }),
        __metadata("design:paramtypes", [HomeService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], HomeComponent);
    return HomeComponent;
}(BaseComponent));
export { HomeComponent };
//# sourceMappingURL=home.component.js.map