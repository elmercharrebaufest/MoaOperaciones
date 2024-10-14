import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MisCursosComponent } from './mis-cursos/mis-cursos.component';
import { AdministrarCursosComponent } from './administrar-cursos/administrar-cursos.component';

const routes: Routes = [
  {
    path: 'mis-cursos',
    component: MisCursosComponent
  },
  {
    path: 'administrar-cursos',
    component: AdministrarCursosComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CursosRoutingModule { }
