import { NgModule } from '@angular/core';
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

@NgModule({
  imports: [
    CommonModule,
    CursosRoutingModule,
    SharedModule,
    NgxPaginationModule,
    ButtonModule,
    DropdownModule,
    ReactiveFormsModule
  ],
  declarations: [MisCursosComponent, AdministrarCursosComponent],
  providers: [ScormService, CursosService]
})
export class CursosModule { }
