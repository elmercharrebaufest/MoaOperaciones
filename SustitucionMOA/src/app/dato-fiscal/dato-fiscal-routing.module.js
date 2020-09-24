var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DatoFiscalBaseComponent } from './dato-fiscal.component';
import { DocumentacionComponent } from './documentacion/documentacion.component';
import { VendedoresListComponent } from './vendedor/dato-fiscal.vendedor.component';
var routes = [
    { path: '', redirectTo: '/situacion-fiscal', pathMatch: 'full' },
    { path: "situacion-fiscal", component: DatoFiscalBaseComponent, },
    { path: "situacion-fiscal/:id", component: DatoFiscalBaseComponent, },
    { path: "situacion-fiscal/:id/:id2", component: DatoFiscalBaseComponent, },
    { path: "documentacion", component: DocumentacionComponent },
    { path: "vendedor", component: VendedoresListComponent },
];
var DatoFiscalRoutingModule = /** @class */ (function () {
    function DatoFiscalRoutingModule() {
    }
    DatoFiscalRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule]
        })
    ], DatoFiscalRoutingModule);
    return DatoFiscalRoutingModule;
}());
export { DatoFiscalRoutingModule };
//# sourceMappingURL=dato-fiscal-routing.module.js.map