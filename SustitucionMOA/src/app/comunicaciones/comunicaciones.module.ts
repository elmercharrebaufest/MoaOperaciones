import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComunicacionesComponent } from './comunicaciones.component';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { ComunicacionesRoutingModule } from './comunicaciones-routing.module';
import { ConsultaModule } from '../consulta/consulta.module';
import { CrearConsultaComponent } from '../consulta/crear-consulta/crear-consulta.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { NgxMaskModule } from 'ngx-mask';
import { MultiSelectModule } from 'primeng/multiselect';
import { CheckboxModule } from 'primeng/checkbox';

@NgModule({
  imports: [
    CommonModule,
    AngularEditorModule,
    FormsModule,
    SharedModule,
    NgxPaginationModule,
    ComunicacionesRoutingModule,

    CommonModule,
    SharedModule,      
    ConfirmDialogModule,
    NgxPaginationModule,
    ButtonModule, DropdownModule, AutoCompleteModule,
    NgxMaskModule,
    MultiSelectModule,
    CheckboxModule,

    ConsultaModule
  ],
  declarations: [
    ComunicacionesComponent,
  ],
  exports: [ComunicacionesComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ComunicacionesModule { }
