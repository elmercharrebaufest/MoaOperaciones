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
import { VendedorStatusService } from './vendedor_status.service';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/Seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { ModalService } from './../common/services/ModalService';
var VendedorStatusComponent = /** @class */ (function (_super) {
    __extends(VendedorStatusComponent, _super);
    function VendedorStatusComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService, route, router) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.route = route;
        _this.router = router;
        _this.cuit = "";
        _this.spinnerComponent = new SpinnerComponent();
        _this.mensajeComponent = new MensajeComponent();
        return _this;
    }
    VendedorStatusComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("vendedor", "Vendedor Estado");
    };
    VendedorStatusComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR VENDEDOR STATUS");
        //this.service.getTitulo().subscribe(titulo => this.titulo = titulo);
        var secciones = [];
        secciones.push(new Seccion('/vendedor/status', 'vendedor', 'Vendedor Estado'));
        this.navService.setSeccionList(secciones);
    };
    VendedorStatusComponent.prototype.getData = function () {
        var _this = this;
        this.datosFiscales = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.route.params.forEach(function (params) {
            _this.subscription = _this.service.getVendedorStatus(_this.cuit).subscribe(function (result) {
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
                    _this.datosFiscales = result.data;
                    if (_this.datosFiscales.status.toLowerCase().indexOf("inhabilitado") >= 0) {
                        _this.statusHabilitado = false;
                        _this.statusInhabilitado = true;
                        _this.statusObservado = false;
                    }
                    else if (_this.datosFiscales.status.toLowerCase().indexOf("habilitado") >= 0) {
                        _this.statusHabilitado = true;
                        _this.statusInhabilitado = false;
                        _this.statusObservado = false;
                    }
                    else {
                        _this.statusHabilitado = false;
                        _this.statusInhabilitado = false;
                        _this.statusObservado = true;
                    }
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        });
        return false;
    };
    VendedorStatusComponent.prototype.isVencida = function (fechaHasta) {
        if (fechaHasta != undefined) {
            return new Date(fechaHasta) < new Date();
        }
        return false;
    };
    VendedorStatusComponent.prototype.showModalTableExencionesResponsive = function (Exencion) {
        this.modalService.openModalTableResponsive("Exención", [
            { etiqueta: "Descripción", valor: Exencion.descripcion },
            { etiqueta: "% Exención", valor: Exencion.exencion },
            { etiqueta: "Fecha Desde", valor: Exencion.fechaDesde },
            { etiqueta: "Fecha Hasta", valor: Exencion.fechaHasta },
        ]);
        return false;
    };
    VendedorStatusComponent.prototype.showModalTableConveniosMultilateralesResponsive = function (Convenio) {
        this.modalService.openModalTableResponsive("Convenio Multilateral", [
            { etiqueta: "Provincia", valor: Convenio.provincia },
            { etiqueta: "Coeficiente", valor: Convenio.coeficiente },
            { etiqueta: "Descripcion", valor: Convenio.descripcion },
        ]);
        return false;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], VendedorStatusComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], VendedorStatusComponent.prototype, "spinnerComponent", void 0);
    VendedorStatusComponent = __decorate([
        Component({
            selector: 'app-vendedor-status',
            //template: '<h1>{{titulo}}</h1>'
            templateUrl: "vendedor_status.component.html",
            providers: [VendedorStatusService]
        }),
        __metadata("design:paramtypes", [VendedorStatusService,
            NavService,
            SecurityService,
            SessionDataService,
            FloatMsgService,
            ModalService,
            ActivatedRoute,
            Router])
    ], VendedorStatusComponent);
    return VendedorStatusComponent;
}(BaseComponent));
export { VendedorStatusComponent };
//# sourceMappingURL=vendedor_status.component.js.map