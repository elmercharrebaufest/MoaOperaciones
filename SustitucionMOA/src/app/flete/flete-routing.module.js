var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FleteAFacturarComponent } from "./a-facturar/flete.a-facturar.component";
import { FleteFacturadoComponent } from "./facturado/flete.facturado.component";
import { FletePendienteComponent } from "./pendiente/flete.pendiente.component";
var routes = [
    { path: '', component: FleteAFacturarComponent },
    { path: "a-facturar", component: FleteAFacturarComponent },
    { path: "facturado", component: FleteFacturadoComponent },
    { path: "pendiente", component: FletePendienteComponent },
];
var FleteRoutingModule = /** @class */ (function () {
    function FleteRoutingModule() {
    }
    FleteRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule]
        })
    ], FleteRoutingModule);
    return FleteRoutingModule;
}());
export { FleteRoutingModule };
//# sourceMappingURL=flete-routing.module.js.map