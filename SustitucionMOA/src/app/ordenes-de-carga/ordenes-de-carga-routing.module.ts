import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OrdenesDeCargaAlta } from './alta/ordenes-de-carga.alta.component';
import { OrdenesDeCargaDetalleComponent } from './detalle/ordenes-de-carga.detalle.component';
import { OrdenesDeCargaListado } from './listado/ordenes-de-carga.listado.component';

const routes: Routes = [
  { path: '', component: OrdenesDeCargaListado },
  { path: "alta/:tipoOperacion", component: OrdenesDeCargaAlta},
  { path: "alta/:id/:tipoOperacion", component: OrdenesDeCargaAlta },
  { path: "detalle/:id/:tipoOperacion", component: OrdenesDeCargaDetalleComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class OrdenesDeCargaRoutingModule { }
