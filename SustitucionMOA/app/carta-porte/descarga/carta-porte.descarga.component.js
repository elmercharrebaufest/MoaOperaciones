"use strict";
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
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var carta_porte_component_1 = require("./../carta-porte.component");
var carta_porte_service_1 = require("./../carta-porte.service");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var ModalService_1 = require("./../../common/services/ModalService");
var CartaPorteDescargaComponent = /** @class */ (function (_super) {
    __extends(CartaPorteDescargaComponent, _super);
    function CartaPorteDescargaComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivo = "ReporteDescargas.xls";
        _this.tituloZip = "FotosCartaPorte.zip";
        _this.cartaPorteIDStr = "";
        _this.CartasPortesIDDescarga = {};
        return _this;
    }
    CartaPorteDescargaComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR CARTAS PORTE"); };
    CartaPorteDescargaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("carta-porte", "Descargas");
    };
    CartaPorteDescargaComponent.prototype.showModalTableResponsive = function (recepcionInfo) {
        this.modalService.openModalTableResponsive("Carta de Porte", [
            { etiqueta: "Fecha", valor: recepcionInfo.fechaDescarga },
            { etiqueta: "CCPP Nº", valor: recepcionInfo.cartaPorte },
            { etiqueta: "Producto", valor: recepcionInfo.producto },
            { etiqueta: "Contrato", valor: recepcionInfo.contrnum },
            { etiqueta: "Recibido", valor: recepcionInfo.netoDescontadoString },
            { etiqueta: "Vendedor", valor: recepcionInfo.vendedor }
        ]);
        return false;
    };
    CartaPorteDescargaComponent.prototype.validarCheckboxesFotos = function () {
        var _this = this;
        this.cartaPorteIDStr = Object.keys(this.CartasPortesIDDescarga).filter(function (item, index) {
            return _this.CartasPortesIDDescarga[item];
        }).join(",");
        if (this.cartaPorteIDStr == "") {
            this.mensajeComponent.setMsgsEmpty();
            this.mensajeComponent.setInfoMsg("Debe seleccionar las cartas de porte que quiere descargar");
        }
        else {
            console.log(this.cartaPorteIDStr);
            this.descargarFotos();
        }
    };
    CartaPorteDescargaComponent.prototype.descargarFotos = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.descargarFotos(this.cartaPorteIDStr).subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
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
                var byteArray = new Uint8Array(result.FileContents);
                var blob = new Blob([byteArray], { type: 'application/zip' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloZip);
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloZip;
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false; // <- Prevent href del a
    };
    CartaPorteDescargaComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/carta-porte/descarga/carta-porte.descarga.component.html?v=" + new Date().getTime(),
            providers: [{ provide: carta_porte_service_1.CartaPorteService, useClass: carta_porte_service_1.CartaPorteDescargaService }]
        }),
        __metadata("design:paramtypes", [carta_porte_service_1.CartaPorteService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], CartaPorteDescargaComponent);
    return CartaPorteDescargaComponent;
}(carta_porte_component_1.CartaPorteBaseComponent));
exports.CartaPorteDescargaComponent = CartaPorteDescargaComponent;
//# sourceMappingURL=carta-porte.descarga.component.js.map