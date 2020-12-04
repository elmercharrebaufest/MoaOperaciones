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
import { FleteService, FleteFacturadoService } from './../flete.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
var FleteFacturadoComponent = /** @class */ (function (_super) {
    __extends(FleteFacturadoComponent, _super);
    function FleteFacturadoComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivo = "FletesFacturados.xls";
        return _this;
    }
    FleteFacturadoComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("flete", "Viajes Facturados");
    };
    FleteFacturadoComponent.prototype.ngAfterViewInit = function () {
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
    FleteFacturadoComponent.prototype.showModalTableResponsive = function (Viaje) {
        this.modalService.openModalTableResponsive("Viaje Factuado", [
            { etiqueta: "CCPP", valor: Viaje.ccpp },
            { etiqueta: "Fecha", valor: Viaje.fechaCCPP },
            { etiqueta: "Patente", valor: Viaje.patente },
            { etiqueta: "KG CCPP", valor: Viaje.kgString },
            { etiqueta: "Descripción", valor: Viaje.descMat },
            { etiqueta: "Origen", valor: Viaje.origen },
            { etiqueta: "Destino", valor: Viaje.destino },
            { etiqueta: "Factura Legal Nº", valor: Viaje.factura },
            { etiqueta: "Factura Fecha", valor: Viaje.fechaEmision },
            { etiqueta: "Tarifa", valor: Viaje.tarifaString },
            { etiqueta: "Peaje", valor: Viaje.peajeString },
            { etiqueta: "Playas", valor: Viaje.peajeString },
            { etiqueta: "Importe", valor: Viaje.importeString }
        ]);
        return false;
    };
    FleteFacturadoComponent = __decorate([
        Component({
            selector: 'app-flete-facturado',
            templateUrl: "flete.facturado.component.html",
            providers: [{ provide: FleteService, useClass: FleteFacturadoService }]
        }),
        __metadata("design:paramtypes", [FleteService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], FleteFacturadoComponent);
    return FleteFacturadoComponent;
}(FleteBaseComponent));
export { FleteFacturadoComponent };
//# sourceMappingURL=flete.facturado.component.js.map