var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ReporteContratoComponent } from './contrato/reporte.contrato.component';
import { ReporteCupoComponent } from './cupo/reporte.cupo.component';
var routes = [
    { path: '', component: ReporteContratoComponent },
    { path: 'contrato', component: ReporteContratoComponent },
    { path: "cupo", component: ReporteCupoComponent },
];
var ReporteRoutingModule = /** @class */ (function () {
    function ReporteRoutingModule() {
    }
    ReporteRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule]
        })
    ], ReporteRoutingModule);
    return ReporteRoutingModule;
}());
export { ReporteRoutingModule };
//# sourceMappingURL=reporte-routing.module.js.map