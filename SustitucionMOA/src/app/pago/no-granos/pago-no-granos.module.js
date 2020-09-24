var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { PagoEmitidoNGService, PagoService } from '../pago.service';
import { PagoEmitidoNGSComponent } from './emitido/pago.no-granos.emitido.component';
import { PagoNoGranosRoutingModule } from './pago-no-granos-routing.module';
var PagoNoGranosModule = /** @class */ (function () {
    function PagoNoGranosModule() {
    }
    PagoNoGranosModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                PagoNoGranosRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                PagoEmitidoNGSComponent,
            ],
            providers: [
                PagoService,
                PagoEmitidoNGService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], PagoNoGranosModule);
    return PagoNoGranosModule;
}());
export { PagoNoGranosModule };
//# sourceMappingURL=pago-no-granos.module.js.map