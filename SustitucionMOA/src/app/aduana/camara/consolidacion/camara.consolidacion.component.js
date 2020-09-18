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
import { AduanaService } from './../../aduana.service';
import { AduanaBaseComponent } from './../../aduana.component';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SecurityService } from './../../../common/services/SecurityService';
import { MensajeComponent } from './../../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../../common/view-child/spinner/spinner.component';
import { ModalService } from './../../../common/services/ModalService';
var CamaraConsolidacionComponent = /** @class */ (function (_super) {
    __extends(CamaraConsolidacionComponent, _super);
    function CamaraConsolidacionComponent(navService, service, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, service, securityService, floatMsgService, modalService) || this;
        _this.navService = navService;
        _this.service = service;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.camaraActualNombre = "";
        _this.camaraUrlActual = "";
        _this.camaraImagen = "";
        _this.cambiandoCamara = false;
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    CamaraConsolidacionComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR CAMARAS CONSOLIDACIO"); };
    CamaraConsolidacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("aduana", "Cámaras Consolidación");
    };
    CamaraConsolidacionComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.cambiarCamara("http://10.10.115.21/jpg/image.jpg", "BZA-CABEZAL");
    };
    CamaraConsolidacionComponent.prototype.cambiarCamara = function (url, nombre) {
        var _this = this;
        this.camaraActualNombre = nombre;
        this.camaraUrlActual = url;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.cambiandoCamara = true;
        this.unsubscribe();
        this.subscription = this.service.getImagenCamaraConsolidacion(url, nombre).subscribe(function (result) {
            if (_this.camaraActualNombre == result.nombre) {
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
                    _this.camaraImagen = "data:image/jpg;base64," + result.img;
                    _this.cambiandoCamara = false;
                    _this.obtenerImagen();
                }
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
        });
        return false;
    };
    CamaraConsolidacionComponent.prototype.obtenerImagen = function () {
        var _this = this;
        if (!this.cambiandoCamara) {
            this.unsubscribe();
            this.subscription = this.service.getImagenCamaraConsolidacion(this.camaraUrlActual, this.camaraActualNombre).subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (_this.camaraActualNombre == result.nombre && result.img != undefined && result.img != "") {
                    _this.camaraImagen = "data:image/jpg;base64," + result.img;
                }
                _this.obtenerImagen();
            });
        }
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], CamaraConsolidacionComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], CamaraConsolidacionComponent.prototype, "spinnerComponent", void 0);
    CamaraConsolidacionComponent = __decorate([
        Component({
            selector: 'app-aduana-camara-consolidacion',
            templateUrl: "camara.consolidacion.component.html",
        }),
        __metadata("design:paramtypes", [NavService, AduanaService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], CamaraConsolidacionComponent);
    return CamaraConsolidacionComponent;
}(AduanaBaseComponent));
export { CamaraConsolidacionComponent };
//# sourceMappingURL=camara.consolidacion.component.js.map