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
import { LiquidacionAprobadaComponent } from "./aprobada/liquidacion.aprobada.component";
import { LiquidacionObservadaComponent } from "./observada/liquidacion.observada.component";
import { LiquidacionPagaComponent } from "./paga/liquidacion.paga.component";
import { LiquidacionProformaComponent } from "./proforma/liquidacion.proforma.component";
import { LiquidacionAprobadaService, LiquidacionObservadaService, LiquidacionPagaService, LiquidacionProformaService, LiquidacionService } from './liquidacion.service';
import { LiquidacionRoutingModule } from './liquidacion-routing.module';
var LiquidacionModule = /** @class */ (function () {
    function LiquidacionModule() {
    }
    LiquidacionModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                LiquidacionRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                LiquidacionAprobadaComponent,
                LiquidacionObservadaComponent,
                LiquidacionPagaComponent,
                LiquidacionProformaComponent
            ],
            providers: [
                LiquidacionService,
                LiquidacionAprobadaService,
                LiquidacionObservadaService,
                LiquidacionPagaService,
                LiquidacionProformaService,
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], LiquidacionModule);
    return LiquidacionModule;
}());
export { LiquidacionModule };
//# sourceMappingURL=liquidacion.module.js.map