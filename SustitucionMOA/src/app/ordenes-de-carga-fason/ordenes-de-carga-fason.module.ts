import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
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
import { OrdenesDeCargaFasonRoutingModule } from './ordenes-de-carga-fason-routing.module';
import { OrdenesDeCargaFasonAltaComponent } from './alta/ordenes-de-carga-fason.alta.component';
import { OrdenesDeCargaFasonListadoComponent } from './listado/ordenes-de-carga-fason.listado.component';
import { OrdenesDeCargaFasonDetalleComponent } from './detalle/ordenes-de-carga-fason.detalle.component';
import { OrdenesDeCargaFasonService } from './ordenes-de-carga-fason.service';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@NgModule({
  imports: [
    CommonModule,
    OrdenesDeCargaFasonRoutingModule,
    CommonModule,
    SharedModule,
    NgxPaginationModule,
    ButtonModule,
    DropdownModule,
    AutoCompleteModule,
    NgxSpinnerModule,
    NgxMaskModule,
    MultiSelectModule,
    ConfirmDialogModule,
    CalendarModule,
    CheckboxModule,
    ProgressSpinnerModule,
    DialogModule,
    TooltipModule,
    ToastModule
  ],
  declarations: [
    OrdenesDeCargaFasonAltaComponent,
    OrdenesDeCargaFasonListadoComponent,
    OrdenesDeCargaFasonDetalleComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    OrdenesDeCargaFasonService,
    MessageService
  ],
})

export class OrdenesDeCargaFasonModule { }
