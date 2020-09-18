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
import { EmpresaGranosService } from '../empresa-granos/empresa-granos.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { BaseComponent } from '../../common/base-components/base-component';
var ProveedorDetalleComponent = /** @class */ (function (_super) {
    __extends(ProveedorDetalleComponent, _super);
    function ProveedorDetalleComponent(service, navService, sessionDataService, securytiService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securytiService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securytiService = securytiService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        return _this;
    }
    ProveedorDetalleComponent.prototype.ngOnInit = function () {
    };
    ProveedorDetalleComponent = __decorate([
        Component({
            selector: 'app-proveedor-detalle',
            templateUrl: 'proveedor-detalle.component.html',
            styleUrls: ['proveedor-detalle.component.css', '../../../../Content/css/bootstrap.min.css']
        }),
        __metadata("design:paramtypes", [EmpresaGranosService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], ProveedorDetalleComponent);
    return ProveedorDetalleComponent;
}(BaseComponent));
export { ProveedorDetalleComponent };
//# sourceMappingURL=proveedor-detalle.component.js.map