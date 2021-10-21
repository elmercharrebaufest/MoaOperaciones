import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CartaPorteAplicacionComponent } from "./aplicacion/carta-porte.aplicacion2.component";
import { CartaPorteBaseComponent } from "./carta-porte.component";
import { CartaPorteDescargaComponent } from "./descarga/carta-porte.descarga2.component";
import { CartaPorteDetalleComponent } from "./detalle/carta-porte.detalle2.component";
import { CartaPorteFormularioComponent } from "./formulario/carta-porte.formulario.component";

const routes: Routes = [
    { path: '', component: CartaPorteBaseComponent },
    { path: "descarga", component: CartaPorteDescargaComponent },
    {
      path: "aplicacion",
      component: CartaPorteAplicacionComponent,
    },
    {
      path: "aplicacion/:contrato",
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

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CartaPorteRoutingModule { }
