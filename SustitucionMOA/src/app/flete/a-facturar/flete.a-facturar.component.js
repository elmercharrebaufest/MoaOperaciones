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
import { FleteBaseComponent } from './../flete.component';
import { FleteService, FleteAFacturarService } from './../flete.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
var FleteAFacturarComponent = /** @class */ (function (_super) {
    __extends(FleteAFacturarComponent, _super);
    function FleteAFacturarComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivo = "FletesAFacturar.xls";
        return _this;
    }
    FleteAFacturarComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("flete", "Viajes A Facturar");
    };
    FleteAFacturarComponent.prototype.actualizarTarifa = function (proforma, viaje, event) {
        this.floatMsgService.setMsgsEmpty();
        var valor = 0;
        var valorString = "0";
        if (event) {
            //valorString = this.formatearValor(event);
            //valor = parseFloat(valorString); 
            valorString = event.replace(/\./g, "").replace(',', '.');
            valor = parseFloat(valorString);
            if (isNaN(valor)) {
                this.floatMsgService.setErrorMsg("El valor de tarifa no es válido");
                viaje.tarifa = "0";
                valor = 0;
                return;
            }
            viaje.tarifa = valor.toLocaleString("de-DE");
        }
        else {
            viaje.tarifa = "0";
            valor = 0;
        }
        viaje.importe = ((valor / 1000) * viaje.kg) + viaje.peaje + viaje.playa;
        viaje.importeString = viaje.importe.toLocaleString("de-DE");
        proforma.totalImporte = proforma.viajeItem.map(this.importe).reduce(this.suma);
        proforma.totalImporteString = "$" + proforma.totalImporte.toLocaleString("de-DE");
    };
    FleteAFacturarComponent.prototype.importe = function (item) { return item.importe; };
    FleteAFacturarComponent.prototype.suma = function (prev, next) { return prev + next; };
    /*formatearValor(value: string) {

        value = value.replace(",", "--coma--");
        value = value.replace(".", ",");
        value = value.replace("--coma--", ".");

        return value;
    }*/
    FleteAFacturarComponent.prototype.ngAfterViewInit = function () {
        $(document).on("mouseover", '.form_datetime', function () {
            $(".form_datetime").datetimepicker({
                format: 'yyyy-mm-dd',
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
            });
        });
    };
    /*guardarTarifas(viajeProforma: any) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        let file = viajeProforma.pdf;
        viajeProforma.pdf = null;
        this.subscription = this.service.guardarTarifas(viajeProforma, file).subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {

                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );
        return false;
    }*/
    FleteAFacturarComponent.prototype.validarImporte = function (proforma) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.validarImporte(proforma.totalImporte.toString(), proforma.proforma).subscribe(function (result) {
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
                _this.showModalCargaDatos(proforma.proforma);
            }
        }, function (error) {
            _this.floatMsgService.setErrorMsg(error.message);
        });
        return false;
    };
    FleteAFacturarComponent.prototype.showModalTableResponsive = function (Viaje) {
        this.modalService.openModalTableResponsive("Viaje A Facturar", [
            { etiqueta: "CCPP", valor: Viaje.ccpp },
            { etiqueta: "Fecha", valor: Viaje.fechaCCPP },
            { etiqueta: "Patente", valor: Viaje.patente },
            { etiqueta: "KG CCPP", valor: Viaje.kgString },
            { etiqueta: "Descripción", valor: Viaje.descMat },
            { etiqueta: "Origen", valor: Viaje.origen },
            { etiqueta: "Destino", valor: Viaje.destino },
            { etiqueta: "Tarifa", valor: Viaje.tarifa },
            { etiqueta: "Peaje", valor: Viaje.peajeString },
            { etiqueta: "Playas", valor: Viaje.peajeString },
            { etiqueta: "Importe", valor: Viaje.importeString }
        ]);
        return false;
    };
    FleteAFacturarComponent = __decorate([
        Component({
            selector: 'app-flete-a-facturar',
            templateUrl: "flete.a-facturar.component.html",
            providers: [{ provide: FleteService, useClass: FleteAFacturarService }]
        }),
        __metadata("design:paramtypes", [FleteService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], FleteAFacturarComponent);
    return FleteAFacturarComponent;
}(FleteBaseComponent));
export { FleteAFacturarComponent };
//# sourceMappingURL=flete.a-facturar.component.js.map