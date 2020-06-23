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
var router_1 = require("@angular/router");
var factura_service_1 = require("./factura.service");
var spinner_component_1 = require("./../common/view-child/spinner/spinner.component");
var mensaje_component_1 = require("./../common/view-child/mensaje/mensaje.component");
var list_base_component_1 = require("./../common/base-components/list-base-component");
var NavService_1 = require("./../common/services/NavService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var SecurityService_1 = require("./../common/services/SecurityService");
var Seccion_1 = require("./../common/models/Seccion");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var ModalService_1 = require("./../common/services/ModalService");
var spinner_small_component_1 = require("./../common/view-child/spinner-small/spinner-small.component");
var angular2_recaptcha_1 = require("angular2-recaptcha");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var FacturaComponent = /** @class */ (function (_super) {
    __extends(FacturaComponent, _super);
    function FacturaComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.route = route;
        _this.router = router;
        _this.tituloArchivo = "Factura.xls";
        _this.captchaOk = null;
        _this.spinnerSmallComponent = new spinner_small_component_1.SpinnerSmallComponent();
        return _this;
    }
    FacturaComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CARGAR FACT PROV"); };
    FacturaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("factura", "Factura");
    };
    FacturaComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        var secciones = [];
        secciones.push(new Seccion_1.Seccion('/factura', 'factura', 'Factura'));
        this.navService.setSeccionList(secciones);
    };
    FacturaComponent.prototype.subirPDF = function () {
        var _this = this;
        ;
        this.floatMsgService.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        if (this.file == null) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Ingrese un Archivo");
            return false;
        }
        if (this.captchaOk == null) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Debe completar el Captcha");
            return false;
        }
        this.unsubscribe();
        try {
            this.subscription = this.service.subirPDF(this.file).subscribe(function (result) {
                _this.spinnerSmallComponent.hideIt();
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
                    _this.vaciarCampos();
                    _this.floatMsgService.setSuccessMsg(result.data);
                }
                return false;
            }, function (error) {
                var errormsj = "Ha ocurrido un error, por favor intentelo nuevamente";
                if (error._body.indexOf("length exceeded") >= 0) {
                    errormsj = "El tamaño del archivo supera los 3 MBs permitidos";
                }
                _this.spinnerSmallComponent.hideIt();
                _this.floatMsgService.setErrorMsg(errormsj);
            });
        }
        catch (e) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    FacturaComponent.prototype.cargarArchivo = function (event) {
        var fileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
        }
    };
    FacturaComponent.prototype.handleCorrectCaptcha = function (event) {
        this.captchaOk = event;
    };
    FacturaComponent.prototype.vaciarCampos = function () {
        this.file = null;
        this.fileInput.nativeElement.value = "";
    };
    FacturaComponent.prototype.ngOnDestroy = function () {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        if (this.modalServiceSusbcription != undefined) {
            this.modalServiceSusbcription.unsubscribe();
        }
    };
    __decorate([
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], FacturaComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_small_component_1.SpinnerSmallComponent),
        __metadata("design:type", spinner_small_component_1.SpinnerSmallComponent)
    ], FacturaComponent.prototype, "spinnerSmallComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], FacturaComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        core_1.ViewChild('fileInput'),
        __metadata("design:type", core_1.ElementRef)
    ], FacturaComponent.prototype, "fileInput", void 0);
    __decorate([
        core_1.ViewChild('recaptchaComponent'),
        __metadata("design:type", angular2_recaptcha_1.ReCaptchaComponent)
    ], FacturaComponent.prototype, "captcha", void 0);
    FacturaComponent = __decorate([
        core_1.Component({
            selector: 'app-factura',
            templateUrl: "./app/factura/factura.component.html?v=" + new Date().getTime(),
            providers: [factura_service_1.FacturaService]
        }),
        __metadata("design:paramtypes", [factura_service_1.FacturaService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService, router_1.ActivatedRoute, router_1.Router])
    ], FacturaComponent);
    return FacturaComponent;
}(list_base_component_1.ListBaseComponent));
exports.FacturaComponent = FacturaComponent;
//# sourceMappingURL=factura.component.js.map