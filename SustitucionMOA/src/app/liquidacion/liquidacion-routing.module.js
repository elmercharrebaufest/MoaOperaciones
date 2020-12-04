var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LiquidacionAprobadaComponent } from "./aprobada/liquidacion.aprobada.component";
import { LiquidacionObservadaComponent } from "./observada/liquidacion.observada.component";
import { LiquidacionPagaComponent } from "./paga/liquidacion.paga.component";
import { LiquidacionProformaComponent } from "./proforma/liquidacion.proforma.component";
var routes = [
    { path: '', component: LiquidacionAprobadaComponent },
    { path: "aprobada", component: LiquidacionAprobadaComponent },
    {
        path: "observada",
        component: LiquidacionObservadaComponent,
    },
    { path: "paga", component: LiquidacionPagaComponent },
    {
        path: "proforma/:id",
        component: LiquidacionProformaComponent,
    }
];
var LiquidacionRoutingModule = /** @class */ (function () {
    function LiquidacionRoutingModule() {
    }
    LiquidacionRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule]
        })
    ], LiquidacionRoutingModule);
    return LiquidacionRoutingModule;
}());
export { LiquidacionRoutingModule };
//# sourceMappingURL=liquidacion-routing.module.js.map