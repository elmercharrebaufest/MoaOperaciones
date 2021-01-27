import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OrdenesDeCargaAlta } from './alta/ordenes-de-carga.alta.component';
import { OrdenesDeCargaListado } from './listado/ordenes-de-carga.listado.component';

const routes: Routes = [
  { path: '', component: OrdenesDeCargaListado },
    { path: "alta", component: OrdenesDeCargaAlta},
      { path: "alta/:id", component: OrdenesDeCargaAlta },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class OrdenesDeCargaRoutingModule { }
