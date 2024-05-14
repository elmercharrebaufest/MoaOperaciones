import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CursosRoutingModule } from './cursos-routing.module';
import { MisCursosComponent } from './mis-cursos/mis-cursos.component';
import { AdministrarCursosComponent } from './administrar-cursos/administrar-cursos.component';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { ReactiveFormsModule } from '@angular/forms';
import { ScormService } from './scorm.service';
import { CursosService } from './cursos.service';
import { AsignarCursosComponent } from './acciones/asignar-cursos/asignar-cursos.component';
import { VerProgresoCursoComponent } from './acciones/ver-progreso-curso/ver-progreso-curso.component';
import { VerProgresoAlumnosComponent } from './acciones/ver-progreso-alumnos/ver-progreso-alumnos.component';
import { DialogModule } from 'primeng/dialog';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';

@NgModule({
  imports: [
    CommonModule,
    CursosRoutingModule,
    SharedModule,
    NgxPaginationModule,
    ButtonModule,
    DropdownModule,
    ReactiveFormsModule,
    DialogModule,
    CheckboxModule,
    InputTextModule,
    TooltipModule
  ],
  declarations: [MisCursosComponent, AdministrarCursosComponent, AsignarCursosComponent, VerProgresoCursoComponent, VerProgresoAlumnosComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [ScormService, CursosService]
})
export class CursosModule { }
