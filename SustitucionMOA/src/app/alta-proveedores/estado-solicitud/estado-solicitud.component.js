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
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { EstadoSolicitudService } from './estado-solicitud.service';
var EstadoSolicitudComponent = /** @class */ (function (_super) {
    __extends(EstadoSolicitudComponent, _super);
    function EstadoSolicitudComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        return _this;
    }
    EstadoSolicitudComponent.prototype.ngOnInit = function () {
        this.obtenerEstado();
    };
    EstadoSolicitudComponent.prototype.obtenerEstado = function () {
        var _this = this;
        this.subscription = this.service.getEstadoAprobacion().subscribe(function (result) {
            _this.estadoSolicitud = result.data.EstadoDescripcion;
            _this.observaciones = result.data.Observaciones;
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    EstadoSolicitudComponent = __decorate([
        Component({
            selector: 'app-estado-solicitud',
            templateUrl: 'estado-solicitud.component.html',
            styleUrls: ['estado-solicitud.component.css', '../../../../Content/css/bootstrap.min.css']
        }),
        __metadata("design:paramtypes", [EstadoSolicitudService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], EstadoSolicitudComponent);
    return EstadoSolicitudComponent;
}(ListBaseComponent));
export { EstadoSolicitudComponent };
//# sourceMappingURL=estado-solicitud.component.js.map