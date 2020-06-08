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
var list_base_component_1 = require("./../common/base-components/list-base-component");
var pesificacion_service_1 = require("./pesificacion.service");
var router_1 = require("@angular/router");
var NavService_1 = require("./../common/services/NavService");
var SecurityService_1 = require("./../common/services/SecurityService");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var ModalService_1 = require("./../common/services/ModalService");
var mensaje_component_1 = require("./../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../common/view-child/spinner/spinner.component");
var spinner_small_component_1 = require("./../common/view-child/spinner-small/spinner-small.component");
var PesificacionComponent = /** @class */ (function (_super) {
    __extends(PesificacionComponent, _super);
    function PesificacionComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.route = route;
        _this.router = router;
        _this.fecha = null;
        _this.contrato = "";
        _this.fijacion = "";
        _this.cantidad = 0;
        _this.file = null;
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
        _this.spinnerSmallComponent = new spinner_small_component_1.SpinnerSmallComponent();
        return _this;
    }
    PesificacionComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.initForm();
    };
    PesificacionComponent.prototype.setTabs = function () {
        this.navService.setSeccionList([]);
        this.setMenuSeccionTab("pesificacion", "");
    };
    PesificacionComponent.prototype.checkPermisos = function () {
        this.securityService.tienePermisoRedirect("PESIFICACION");
    };
    PesificacionComponent.prototype.initForm = function () {
        var _this = this;
        this.fecha = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getData().subscribe(function (result) {
            _this.fecha = null;
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
                _this.fecha = result;
                _this.contrato = "";
                _this.fijacion = "";
                _this.cantidad = 0;
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
            return false;
        });
        return false;
    };
    PesificacionComponent.prototype.isVisible = function () {
        if (this.fecha && this.fecha != null)
            return true;
        else
            return false;
    };
    PesificacionComponent.prototype.guardarPesificaciones = function () {
        var _this = this;
        this.spinnerSmallComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        if (this.contrato == null || this.contrato == "") {
            this.spinnerSmallComponent.hideIt();
            this.mensajeComponent.setErrorMsg("Debe ingresar un contrato");
            return false;
        }
        if (this.cantidad == null || this.cantidad <= 0) {
            this.spinnerSmallComponent.hideIt();
            this.mensajeComponent.setErrorMsg("Debe ingresar una cantidad mayor a 0");
            return false;
        }
        this.unsubscribe();
        this.subscription = this.service.setData(this.contrato, this.fijacion, this.cantidad).subscribe(function (result) {
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
                _this.mensajeComponent.setSuccessMsg("Operacion realizada exitosamente");
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
            return false;
        });
        return false;
    };
    PesificacionComponent.prototype.cargarArchivo = function (event) {
        var fileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
        }
    };
    PesificacionComponent.prototype.cargaMasiva = function () {
        var _this = this;
        this.spinnerSmallComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        if (this.file == null || !this.esCSV(this.file.name)) {
            this.spinnerSmallComponent.hideIt();
            this.mensajeComponent.setErrorMsg("Debe seleccionar un archivo .csv valido");
            return false;
        }
        this.unsubscribe();
        this.subscription = this.service.setMassiveData(this.file).subscribe(function (result) {
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
                _this.mensajeComponent.setSuccessMsg("Operacion realizada exitosamente");
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
            return false;
        });
        return false;
    };
    PesificacionComponent.prototype.esCSV = function (name) {
        var ext = name.substring(name.lastIndexOf('.') + 1);
        if (ext.toLowerCase() == 'csv')
            return true;
        else
            return false;
    };
    __decorate([
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], PesificacionComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], PesificacionComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_small_component_1.SpinnerSmallComponent),
        __metadata("design:type", spinner_small_component_1.SpinnerSmallComponent)
    ], PesificacionComponent.prototype, "SpinnerSmallComponent", void 0);
    PesificacionComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: './app/pesificacion/pesificacion.component.html',
            providers: [pesificacion_service_1.PesificacionService]
        }),
        __metadata("design:paramtypes", [pesificacion_service_1.PesificacionService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService, router_1.ActivatedRoute, router_1.Router])
    ], PesificacionComponent);
    return PesificacionComponent;
}(list_base_component_1.ListBaseComponent));
exports.PesificacionComponent = PesificacionComponent;
//# sourceMappingURL=pesificacion.component.js.map