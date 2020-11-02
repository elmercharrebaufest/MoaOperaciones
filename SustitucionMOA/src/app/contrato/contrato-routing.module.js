var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ContratoVigenteComponent } from './vigente/contrato.vigente.component';
import { ContratoAmpliacionComponent } from './ampliacion/contrato.ampliacion.component';
import { ContratoFijacionComponent } from './fijacion/contrato.fijacion.component';
import { ContratoAnulacionComponent } from './anulacion/contrato.anulacion.component';
import { ContratoDetalleComponent } from './detalle/contrato.detalle.component';
import { ContratoDetalleFijacionComponent } from './detalle-fijacion/contrato.detalle-fijacion.component';
import { CrearContratoAPrecioComponent } from '../crear-contrato/aprecio/crear-contrato.aprecio.component';
import { CrearContratoAFijarComponent } from '../crear-contrato/afijar/crear-contrato.afijar.component';
import { CrearContratoBaseComponent } from '../crear-contrato/crear-contrato.component';
var routes = [
    { path: '', component: ContratoVigenteComponent },
    { path: 'vigente', component: ContratoVigenteComponent },
    { path: "fijacion", component: ContratoFijacionComponent },
    { path: "ampliacion", component: ContratoAmpliacionComponent },
    { path: "anulacion", component: ContratoAnulacionComponent },
    { path: "detalle/:id", component: ContratoDetalleComponent },
    { path: "detalle-fijacion/:id/:id2", component: ContratoDetalleFijacionComponent },
    { path: "crear", component: CrearContratoBaseComponent },
    { path: "crear/aprecio", component: CrearContratoAPrecioComponent },
    { path: "crear/afijar", component: CrearContratoAFijarComponent },
];
var ContratoRoutingModule = /** @class */ (function () {
    function ContratoRoutingModule() {
    }
    ContratoRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule]
        })
    ], ContratoRoutingModule);
    return ContratoRoutingModule;
}());
export { ContratoRoutingModule };
//# sourceMappingURL=contrato-routing.module.js.map