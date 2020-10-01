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
import { CartaPorteBaseComponent } from './../carta-porte.component';
import { CartaPorteService, CartaPorteAplicacionService } from './../carta-porte2.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
var CartaPorteAplicacionComponent = /** @class */ (function (_super) {
    __extends(CartaPorteAplicacionComponent, _super);
    function CartaPorteAplicacionComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivo = "ReporteAplicaciones.xls";
        return _this;
    }
    CartaPorteAplicacionComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR CARTAS PORTE"); };
    CartaPorteAplicacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("carta-porte", "Aplicaciones");
    };
    CartaPorteAplicacionComponent.prototype.showModalTableResponsive = function (recepcionInfo) {
        this.modalService.openModalTableResponsive("Carta de Porte", [
            { etiqueta: "Fecha", valor: recepcionInfo.fechaDescarga },
            { etiqueta: "CCPP Nº", valor: recepcionInfo.cartaPorte },
            { etiqueta: "Producto", valor: recepcionInfo.producto },
            { etiqueta: "Recibido", valor: recepcionInfo.netoDescontadoString },
            { etiqueta: "Aplicado", valor: recepcionInfo.aLiquidarString },
            { etiqueta: "Contrato Molinos", valor: recepcionInfo.contrnum },
            { etiqueta: "Contrato Proveedor", valor: recepcionInfo.contrvend },
            { etiqueta: "Vendedor", valor: recepcionInfo.vendedor }
        ]);
        return false;
    };
    CartaPorteAplicacionComponent = __decorate([
        Component({
            selector: 'app-carta-porte-aplicacion',
            templateUrl: "carta-porte.aplicacion.component.html",
            providers: [{ provide: CartaPorteService, useClass: CartaPorteAplicacionService }]
        }),
        __metadata("design:paramtypes", [CartaPorteService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], CartaPorteAplicacionComponent);
    return CartaPorteAplicacionComponent;
}(CartaPorteBaseComponent));
export { CartaPorteAplicacionComponent };
//# sourceMappingURL=carta-porte.aplicacion2.component.js.map