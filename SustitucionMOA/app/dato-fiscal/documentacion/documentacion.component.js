"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
var dato_fiscal_service_1 = require("./../dato-fiscal.service");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var seccion_1 = require("./../../common/models/seccion");
var base_component_1 = require("./../../common/base-components/base-component");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var DocumentacionComponent = /** @class */ (function (_super) {
    __extends(DocumentacionComponent, _super);
    function DocumentacionComponent(service, navService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        return _this;
    }
    DocumentacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("dato-fiscal", "Documentacion");
    };
    DocumentacionComponent.prototype.ngOnInit = function () {
        //this.service.getTitulo().subscribe(titulo => this.titulo = titulo);
        this.setTabs();
        var secciones = [];
        if (this.isAuthorized("CONSULTAR DATOS FISCALES"))
            secciones.push(new seccion_1.Seccion('/dato-fiscal/situacion-fiscal', 'dato-fiscal', 'Mi Situacion Fiscal'));
        if (this.isAuthorized("CONSULTAR VENDEDORES") && this.isCorredor())
            secciones.push(new seccion_1.Seccion('/dato-fiscal/vendedor', 'dato-fiscal', 'Mis Vendedores'));
        if (this.isAuthorized("CONSULTAR DOCUMENTACION"))
            secciones.push(new seccion_1.Seccion('/dato-fiscal/documentacion', 'dato-fiscal', 'Documentacion'));
        this.navService.setSeccionList(secciones);
    };
    DocumentacionComponent = __decorate([
        core_1.Component({
            selector: 'documentacion',
            templateUrl: "./app/dato-fiscal/documentacion/documentacion.component.html?v=" + new Date().getTime(),
            providers: [dato_fiscal_service_1.DatoFiscalService]
        }),
        __metadata("design:paramtypes", [dato_fiscal_service_1.DatoFiscalService, NavService_1.NavService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], DocumentacionComponent);
    return DocumentacionComponent;
}(base_component_1.BaseComponent));
exports.DocumentacionComponent = DocumentacionComponent;
//# sourceMappingURL=documentacion.component.js.map