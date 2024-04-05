import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { OrdenesResiduosListadoComponent } from "./listado/ordenes-residuos.listado.component";

const rutas: Routes = [
    { path: '', component: OrdenesResiduosListadoComponent }
    // { path: 'alta', component: OrdenesResiduosAltaComponent }
    // { path: 'alta/:id', component: OrdenesResiduosAltaComponent }
    // { path: 'detalle/:id', component: OrdenesResiduosDetalleComponent }
];

@NgModule({
    imports: [RouterModule.forChild(rutas)],
    exports: [RouterModule]
})

export class OrdenesResiduosRoutingModule { }

