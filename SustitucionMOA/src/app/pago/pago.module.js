var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { PagoEmitidoService, PagoService } from './pago.service';
import { PagoRoutingModule } from './pago-routing.module';
import { PagoDetalleComponent } from './detalle/pago.detalle.component';
import { PagoEmitidoComponent } from './emitido/pago.emitido.component';
import { PagoComponent } from './pago.component';
var PagoModule = /** @class */ (function () {
    function PagoModule() {
    }
    PagoModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                PagoRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                PagoDetalleComponent,
                PagoEmitidoComponent,
                PagoComponent
            ],
            providers: [
                PagoService,
                PagoEmitidoService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], PagoModule);
    return PagoModule;
}());
export { PagoModule };
//# sourceMappingURL=pago.module.js.map