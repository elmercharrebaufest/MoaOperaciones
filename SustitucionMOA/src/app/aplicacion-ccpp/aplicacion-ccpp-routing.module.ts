import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListadoComponent } from './listado/listado.component';

const routes: Routes = [
  {
    path: "listado",
    component: ListadoComponent
  },
  {
    path: "masiva",
    component: ListadoComponent
  },
  {
    path: "",
    component: ListadoComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AplicacionCcppRoutingModule { }
