import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListadoComponent } from './listado/listado.component';
import { CargaManual } from './carga-manual/carga-manual.component';
import { MasivaComponent } from './masiva/masiva.component';

const routes: Routes = [
  {
    path: "listado",
    component: ListadoComponent
  },
  {
    path: "masiva",
    component: MasivaComponent
  },
  {
    path: "",
    component: CargaManual
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AplicacionCcppRoutingModule { }
