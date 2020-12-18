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
import { ReporteService } from './reporte.service';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { Seccion } from './../common/models/Seccion';
var ReporteBaseComponent = /** @class */ (function (_super) {
    __extends(ReporteBaseComponent, _super);
    function ReporteBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.data = [];
        _this.datosContrato = new Array();
        _this.proveedores = new Array();
        _this.materiales = [];
        _this.monedas = new Array();
        _this.destinos = new Array();
        _this.campanias = new Array();
        _this.zona = new Array();
        _this.bolsasSelect = new Array();
        _this.bolsasConfirma = new Array();
        _this.bolsasFisico = new Array();
        _this.bolsasCarta = new Array();
        _this.condicionVendedor = new Array();
        _this.condicionFijacion = new Array();
        _this.datosCompraNet = null;
        _this.esCorredorEnDataAgro = false;
        _this.keyword2 = "RazonSocial";
        _this.autocompleteNotFoundText = "No encontrado";
        _this.corredorId = null;
        _this.proveedorId = null;
        return _this;
    }
    ReporteBaseComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR CONTRATOS"); };
    ReporteBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([
            new Seccion('/reporte/contrato', 'reporte', 'Contratos'),
        ]);
    };
    ReporteBaseComponent.prototype.selectEventProveedor = function (item) {
        this.proveedorId = item.Id;
        console.log("prov: ", item.Id);
    };
    ReporteBaseComponent.prototype.onChangeSearchProveedor = function (term) {
        var _this = this;
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.buscarProveedoresConCorredor(term).subscribe(function (result) {
                var resultlist = JSON.parse(result);
                _this.proveedores = resultlist.map(function (prov) {
                    return { Id: prov.Id, RazonSocial: prov.RazonSocial + " (" + prov.Cuit + ")", CUIT: prov.Cuit };
                });
                //this.proveedores = JSON.parse(result);
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
    };
    ReporteBaseComponent.prototype.isVisible = function () {
        if (this.data && this.data.length != 0)
            return true;
        else
            return false;
    };
    ReporteBaseComponent.prototype.isVisibleProveedor = function () {
        return this.esCorredorEnDataAgro == true;
    };
    ReporteBaseComponent.prototype.obtenerMateriales = function () {
        var _this = this;
        this.subscription = this.service.obtenerMateriales().subscribe(function (result) {
            var obj = JSON.parse(result);
            obj.Datos.forEach(function (element) {
                var el = {
                    Id: element.MaterialId.toString(),
                    Descripcion: element.Descripcion
                };
                _this.materiales.push(el);
            });
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    ReporteBaseComponent = __decorate([
        Component({
            selector: 'app-reporte',
            template: "",
            providers: [ReporteService]
        }),
        __metadata("design:paramtypes", [ReporteService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], ReporteBaseComponent);
    return ReporteBaseComponent;
}(ListBaseComponent));
export { ReporteBaseComponent };
//# sourceMappingURL=reporte.component.js.map