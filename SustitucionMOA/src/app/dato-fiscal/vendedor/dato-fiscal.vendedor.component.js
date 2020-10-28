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
import { Component, ViewChild } from "@angular/core";
import { DatoFiscalService } from "./../dato-fiscal.service";
import { MensajeComponent } from "./../../common/view-child/mensaje/mensaje.component";
import { SpinnerComponent } from "./../../common/view-child/spinner/spinner.component";
import { NavService } from "./../../common/services/NavService";
import { FloatMsgService } from "./../../common/services/FloatMsgService";
import { SecurityService } from "./../../common/services/SecurityService";
import { Seccion } from "./../../common/models/seccion";
import { BaseComponent } from "./../../common/base-components/base-component";
import { SessionDataService } from "./../../common/services/SessionDataService";
import { ModalService } from "./../../common/services/ModalService";
var VendedoresListComponent = /** @class */ (function (_super) {
    __extends(VendedoresListComponent, _super);
    function VendedoresListComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.orderedByColumn = "vendedor";
        _this.orderDirection = 1;
        _this.itemsPerPage = 20;
        _this.filtroVendedor = "";
        _this.filtroNroVendedor = "";
        _this.nuevoVendedorCUIT = "";
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        _this.mensajeModalComponent = new MensajeComponent();
        _this.spinnerModalComponent = new SpinnerComponent();
        return _this;
    }
    VendedoresListComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("dato-fiscal", "Mis Vendedores");
    };
    VendedoresListComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR VENDEDORES");
        var secciones = [];
        if (this.isAuthorized("CONSULTAR DATOS FISCALES"))
            secciones.push(new Seccion("/dato-fiscal/situacion-fiscal", "dato-fiscal", "Mi Situacion Fiscal"));
        if (this.isAuthorized("CONSULTAR VENDEDORES"))
            secciones.push(new Seccion("/dato-fiscal/vendedor", "dato-fiscal", "Mis Vendedores"));
        if (this.isAuthorized("CONSULTAR DOCUMENTACION"))
            secciones.push(new Seccion("/dato-fiscal/documentacion", "dato-fiscal", "Documentacion"));
        if (this.isAuthorized("CONSULTAR VENDEDOR PENDIENTES"))
            secciones.push(new Seccion("/dato-fiscal/vendedores-pendientes", "dato-fiscal", "Vendedores pendientes"));
        this.navService.setSeccionList(secciones);
        this.getUsuario();
    };
    VendedoresListComponent.prototype.getUsuario = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.getVendedores("", "").subscribe(function (result) {
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
                    _this.data = result.data.vendedores;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    VendedoresListComponent.prototype.isVisible = function () {
        return this.data && this.data.length != 0;
    };
    VendedoresListComponent.prototype.orderColumnBy = function (column) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        }
        else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    };
    VendedoresListComponent.prototype.solicitarAltaProveedor = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.mensajeModalComponent.setMsgsEmpty();
        this.spinnerModalComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service
                .agregarVendedor(this.nuevoVendedorCUIT)
                .subscribe(function (result) {
                _this.spinnerModalComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined &&
                    result.error != "") {
                    _this.mensajeModalComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeModalComponent.setInfoMsg(result.info);
                }
                else {
                    //this.getVendedores();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.nuevoVendedorCUIT = "";
                    document.getElementById("modalToggleButton").click();
                }
            }, function (error) {
                _this.spinnerModalComponent.hideIt();
                _this.mensajeModalComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerModalComponent.hideIt();
            this.mensajeModalComponent.setErrorMsg(e);
        }
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], VendedoresListComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], VendedoresListComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild("mensajeModal"),
        __metadata("design:type", MensajeComponent)
    ], VendedoresListComponent.prototype, "mensajeModalComponent", void 0);
    __decorate([
        ViewChild("spinnerModal"),
        __metadata("design:type", SpinnerComponent)
    ], VendedoresListComponent.prototype, "spinnerModalComponent", void 0);
    VendedoresListComponent = __decorate([
        Component({
            selector: "list",
            templateUrl: "dato-fiscal.vendedor.component.html",
            styleUrls: ["dato-fiscal.vendedor.component.css"],
            providers: [DatoFiscalService],
        }),
        __metadata("design:paramtypes", [DatoFiscalService,
            NavService,
            SecurityService,
            SessionDataService,
            FloatMsgService,
            ModalService])
    ], VendedoresListComponent);
    return VendedoresListComponent;
}(BaseComponent));
export { VendedoresListComponent };
//# sourceMappingURL=dato-fiscal.vendedor.component.js.map