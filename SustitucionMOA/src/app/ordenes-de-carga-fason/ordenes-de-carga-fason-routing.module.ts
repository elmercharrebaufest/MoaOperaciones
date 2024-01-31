import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OrdenesDeCargaFasonListadoComponent } from './listado/ordenes-de-carga-fason.listado.component';
import { OrdenesDeCargaFasonAltaComponent } from './alta/ordenes-de-carga-fason.alta.component';
import { OrdenesDeCargaFasonDetalleComponent } from './detalle/ordenes-de-carga-fason.detalle.component';


const routes: Routes = [
  { path: '', component: OrdenesDeCargaFasonListadoComponent },
  { path: "alta", component: OrdenesDeCargaFasonAltaComponent },
  { path: "alta/:id", component: OrdenesDeCargaFasonAltaComponent },
  { path: "detalle/:id", component: OrdenesDeCargaFasonDetalleComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class OrdenesDeCargaFasonRoutingModule { }
