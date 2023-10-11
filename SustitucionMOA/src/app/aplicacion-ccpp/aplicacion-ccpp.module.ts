import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NgxMaskModule } from "ngx-mask";
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

import { AplicacionCcppService } from './aplicacion-ccpp.service';
import { AplicacionCcppRoutingModule } from './aplicacion-ccpp-routing.module';
import { ListadoComponent } from './listado/listado.component';
import { ReactiveFormsModule } from '@angular/forms';
import { CargaManual } from './carga-manual/carga-manual.component';
import { TooltipModule } from 'primeng/tooltip';

@NgModule({
  imports: [
    CommonModule,
    AplicacionCcppRoutingModule,
    SharedModule,
    NgxPaginationModule,
    ButtonModule,
    DropdownModule,
    AutoCompleteModule,
    NgxSpinnerModule,
    NgxMaskModule,
    MultiSelectModule,
    ConfirmDialogModule,
    ReactiveFormsModule,
    TooltipModule,
    ToastModule,
  ],
  declarations: [ListadoComponent, CargaManual],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [AplicacionCcppService, MessageService]
})
export class AplicacionCcppModule { }
