var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CrearContratoAPrecioComponent } from './aprecio/crear-contrato.aprecio.component';
import { CrearContratoAFijarComponent } from './afijar/crear-contrato.afijar.component';
import { CrearContratoFijacionComponent } from './fijacion/crear-contrato.fijacion.component';
import { CrearContratoAltaMasivaComponent } from './alta-masiva/crear-contrato.alta-masiva.component';
import { CrearContratoCargarNegocioComponent } from './cargarnegocio/crear-contrato.cargarnegocio.component';
var routes = [
    { path: '', component: CrearContratoAPrecioComponent },
    { path: 'aprecio', component: CrearContratoAPrecioComponent },
    { path: "afijar", component: CrearContratoAFijarComponent },
    { path: "fijacion", component: CrearContratoFijacionComponent },
    { path: "cargarnegocio", component: CrearContratoCargarNegocioComponent },
    { path: 'aprecio/:id', component: CrearContratoAPrecioComponent },
    { path: "afijar/:id", component: CrearContratoAFijarComponent },
    { path: "fijacion/:id", component: CrearContratoFijacionComponent },
    { path: "alta-masiva", component: CrearContratoAltaMasivaComponent },
];
var CrearContratoRoutingModule = /** @class */ (function () {
    function CrearContratoRoutingModule() {
    }
    CrearContratoRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule]
        })
    ], CrearContratoRoutingModule);
    return CrearContratoRoutingModule;
}());
export { CrearContratoRoutingModule };
//# sourceMappingURL=crear-contrato-routing.module.js.map