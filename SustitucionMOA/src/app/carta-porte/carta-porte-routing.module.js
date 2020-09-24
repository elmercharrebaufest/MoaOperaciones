var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CartaPorteAplicacionComponent } from "./aplicacion/carta-porte.aplicacion2.component";
import { CartaPorteBaseComponent } from "./carta-porte.component";
import { CartaPorteDescargaComponent } from "./descarga/carta-porte.descarga2.component";
import { CartaPorteDetalleComponent } from "./detalle/carta-porte.detalle2.component";
import { CartaPorteFormularioComponent } from "./formulario/carta-porte.formulario.component";
var routes = [
    { path: '', component: CartaPorteBaseComponent },
    { path: "descarga", component: CartaPorteDescargaComponent },
    {
        path: "aplicacion",
        component: CartaPorteAplicacionComponent,
    },
    {
        path: "detalle/:id",
        component: CartaPorteDetalleComponent,
    },
    {
        path: "formulario",
        component: CartaPorteFormularioComponent,
    },
];
var CartaPorteRoutingModule = /** @class */ (function () {
    function CartaPorteRoutingModule() {
    }
    CartaPorteRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule]
        })
    ], CartaPorteRoutingModule);
    return CartaPorteRoutingModule;
}());
export { CartaPorteRoutingModule };
//# sourceMappingURL=carta-porte-routing.module.js.map