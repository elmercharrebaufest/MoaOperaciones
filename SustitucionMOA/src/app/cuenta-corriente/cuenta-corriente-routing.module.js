var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CuentaCorrienteAgrupadaComponent } from './agrupada/cuenta-corriente.agrupada.component';
import { CuentaCorrientePartidasAbiertasComponent } from './partidas-abiertas/cuenta-corriente.partidas-abiertas.component';
import { CuentaCorrienteBaseComponent } from './cuenta-corriente.component';
var routes = [
    { path: '', component: CuentaCorrienteBaseComponent },
    {
        path: "simple",
        component: CuentaCorrienteBaseComponent,
    },
    {
        path: "simple/:id",
        component: CuentaCorrienteBaseComponent,
    },
    {
        path: "agrupada",
        component: CuentaCorrienteAgrupadaComponent,
    },
    {
        path: "agrupada/:id",
        component: CuentaCorrienteAgrupadaComponent,
    },
    {
        path: "partidas-abiertas",
        component: CuentaCorrientePartidasAbiertasComponent,
    },
];
var CuentaCorrienteRoutingModule = /** @class */ (function () {
    function CuentaCorrienteRoutingModule() {
    }
    CuentaCorrienteRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule]
        })
    ], CuentaCorrienteRoutingModule);
    return CuentaCorrienteRoutingModule;
}());
export { CuentaCorrienteRoutingModule };
//# sourceMappingURL=cuenta-corriente-routing.module.js.map