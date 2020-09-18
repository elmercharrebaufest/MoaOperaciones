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
import { RYDService } from './ryd.service';
//import { PaginationControlsCustomComponent } from './../common/view-child/pagination/pagination.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { ModalService } from './../common/services/ModalService';
var RYDBaseComponent = /** @class */ (function (_super) {
    __extends(RYDBaseComponent, _super);
    function RYDBaseComponent(navService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        return _this;
    }
    RYDBaseComponent.prototype.checkPermisos = function () { };
    RYDBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/ryd/carga-pesada', 'ryd', 'Carga de Pesadas'), new Seccion('/ryd/informe', 'ryd', 'Informe'), new Seccion('/ryd/listado-pesadas', 'ryd', 'Listado de Pesadas')]);
    };
    RYDBaseComponent = __decorate([
        Component({
            selector: 'app-ryd',
            template: "",
            providers: [RYDService]
        }),
        __metadata("design:paramtypes", [NavService, SecurityService, FloatMsgService, ModalService])
    ], RYDBaseComponent);
    return RYDBaseComponent;
}(BaseComponent));
export { RYDBaseComponent };
//# sourceMappingURL=ryd.component.js.map