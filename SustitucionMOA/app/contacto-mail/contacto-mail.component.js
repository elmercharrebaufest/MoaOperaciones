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
var contacto_mail_service_1 = require("./contacto-mail.service");
var list_base_component_1 = require("./../common/base-components/list-base-component");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var SecurityService_1 = require("./../common/services/SecurityService");
var dropdown_component_1 = require("./../common/view-child/dropdown/dropdown.component");
var spinner_small_component_1 = require("./../common/view-child/spinner-small/spinner-small.component");
var NavService_1 = require("./../common/services/NavService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var ModalService_1 = require("./../common/services/ModalService");
var angular2_recaptcha_1 = require("angular2-recaptcha");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var ContactoMailComponent = /** @class */ (function (_super) {
    __extends(ContactoMailComponent, _super);
    function ContactoMailComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.route = route;
        _this.router = router;
        _this.categoriaOptions = [];
        _this.camposAdicionales = false;
        _this.visibleButton = true;
        _this.captchaOk = null;
        _this.categoriaDropdownComponent = new dropdown_component_1.DropdownComponent();
        _this.spinnerSmallComponent = new spinner_small_component_1.SpinnerSmallComponent();
        return _this;
    }
    ContactoMailComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); };
    ContactoMailComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contacto", "Contacto");
    };
    ContactoMailComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        //this.getData();
    };
    ContactoMailComponent.prototype.ngAfterViewInit = function () {
        this.getCategorias();
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
    ContactoMailComponent.prototype.sendContactoMail = function () {
        var _this = this;
        ;
        this.floatMsgService.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        try {
            this.categoriaSelected.label;
        }
        catch (_a) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Debe seleccionar una Categoria");
            return false;
        }
        if (this.captchaOk == null) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Debe completar el Captcha");
            return false;
        }
        try {
            var fecha = this.fechaPagoDTP.nativeElement.value;
        }
        catch (_b) { }
        this.unsubscribe();
        try {
            this.subscription = this.service.sendContactoMail(this.proveedor, this.nombre, this.email, this.telefono, this.categoriaSelected.label, this.categoriaSelected.camposAdicionales, this.comentario, this.contrato, this.razonSocial, this.cuit, this.nombreVendedor, this.comprobante, fecha, this.importe, this.impuesto, this.inscripcion, this.motivo, this.file).subscribe(function (result) {
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
                    _this.captcha.reset();
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
    ContactoMailComponent.prototype.setCategoria = function (categoria) {
        this.categoriaSelected = this.categoriaOptions.filter(function (x) { return x.value == categoria; })[0];
        if (this.categoriaSelected != null && this.categoriaSelected != undefined) {
            if (this.categoriaSelected.camposAdicionales === "A") {
                this.camposAdicionales = true;
            }
            else {
                this.camposAdicionales = false;
                this.vaciarCamposAdicionales();
            }
        }
    };
    ContactoMailComponent.prototype.cargarArchivo = function (event) {
        var fileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
        }
    };
    ContactoMailComponent.prototype.vaciarCamposAdicionales = function () {
        this.contrato = '';
        this.razonSocial = '';
        this.cuit = '';
        this.nombreVendedor = '';
        this.comprobante = '';
        this.fechaPago = '';
        this.importe = '';
        this.impuesto = '';
        this.inscripcion = '';
        this.motivo = '';
    };
    ContactoMailComponent.prototype.vaciarCampos = function () {
        this.camposAdicionales = false;
        this.vaciarCamposAdicionales();
        this.proveedor = "";
        this.nombre = "";
        this.email = "";
        this.telefono = "";
        this.categoriaDropdownComponent.setSelectItem("");
        this.comentario = "";
        this.file = null;
        this.fileInput.nativeElement.value = "";
    };
    ContactoMailComponent.prototype.getCategorias = function () {
        var _this = this;
        this.unsubscribe();
        try {
            this.subscription = this.service.getCategorias().subscribe(function (result) {
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
                    _this.categoriaOptions = result.data;
                }
            }, function (error) {
                _this.floatMsgService.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    ContactoMailComponent.prototype.handleCorrectCaptcha = function (event) {
        this.captchaOk = event;
    };
    __decorate([
        core_1.ViewChild('dropdown_categoria'),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], ContactoMailComponent.prototype, "categoriaDropdownComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_small_component_1.SpinnerSmallComponent),
        __metadata("design:type", spinner_small_component_1.SpinnerSmallComponent)
    ], ContactoMailComponent.prototype, "spinnerSmallComponent", void 0);
    __decorate([
        core_1.ViewChild('fileInput'),
        __metadata("design:type", core_1.ElementRef)
    ], ContactoMailComponent.prototype, "fileInput", void 0);
    __decorate([
        core_1.ViewChild('dtp_fecha_pago'),
        __metadata("design:type", core_1.ElementRef)
    ], ContactoMailComponent.prototype, "fechaPagoDTP", void 0);
    __decorate([
        core_1.ViewChild('recaptchaComponent'),
        __metadata("design:type", angular2_recaptcha_1.ReCaptchaComponent)
    ], ContactoMailComponent.prototype, "captcha", void 0);
    ContactoMailComponent = __decorate([
        core_1.Component({
            selector: 'app-contacto-mail',
            templateUrl: "./app/contacto-mail/contacto-mail.component.html?v=" + new Date().getTime(),
            providers: [contacto_mail_service_1.ContactoMailService]
        }),
        __metadata("design:paramtypes", [contacto_mail_service_1.ContactoMailService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService, router_1.ActivatedRoute, router_1.Router])
    ], ContactoMailComponent);
    return ContactoMailComponent;
}(list_base_component_1.ListBaseComponent));
exports.ContactoMailComponent = ContactoMailComponent;
//# sourceMappingURL=contacto-mail.component.js.map