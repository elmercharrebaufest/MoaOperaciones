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
import { LiquidacionNGAprobadaComponent } from "./aprobada/liquidacion.no-granos.aprobada.component";
import { LiquidacionNGObservadaComponent } from "./observada/liquidacion.no-granos.observada.component";
import { LiquidacionNoGranosRoutingModule } from './liquidacion-no-granos-routing.module';
import { LiquidacionNGAprobadaService, LiquidacionNGObservadaService, LiquidacionNGPagaService, LiquidacionService } from '../liquidacion.service';
var LiquidacionNoGranosModule = /** @class */ (function () {
    function LiquidacionNoGranosModule() {
    }
    LiquidacionNoGranosModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                LiquidacionNoGranosRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                LiquidacionNGAprobadaComponent,
                LiquidacionNGObservadaComponent
            ],
            providers: [
                LiquidacionService,
                LiquidacionNGAprobadaService,
                LiquidacionNGObservadaService,
                LiquidacionNGPagaService,
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], LiquidacionNoGranosModule);
    return LiquidacionNoGranosModule;
}());
export { LiquidacionNoGranosModule };
//# sourceMappingURL=liquidacion-no-granos.module.js.map