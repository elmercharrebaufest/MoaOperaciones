import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CursosRoutingModule } from './cursos-routing.module';
import { MisCursosComponent } from './mis-cursos/mis-cursos.component';
import { AdministrarCursosComponent } from './administrar-cursos/administrar-cursos.component';

@NgModule({
  imports: [
    CommonModule,
    CursosRoutingModule
  ],
  declarations: [MisCursosComponent, AdministrarCursosComponent]
})
export class CursosModule { }
