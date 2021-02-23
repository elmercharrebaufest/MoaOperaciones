import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
//import { LiquidacionAprobadaService, LiquidacionObservadaService, LiquidacionPagaService, LiquidacionProformaService, LiquidacionService } from './liquidacion.service';
import { ConsultaRoutingModule } from './consulta-routing.module';
import { ConsultaService } from './consulta.service';
import { CrearConsultaComponent } from './crear-consulta/crear-consulta.component';
import { MisConsultasComponent } from './mis-consultas/mis-consultas.component';
import { DetalleConsultaComponent } from './detalle/consulta-detalle.component';
import { TableModule } from 'primeng/table';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { SliderModule } from 'primeng/slider';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { SpinnerModule } from 'primeng/spinner';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { OverlayPanelModule } from 'primeng/overlaypanel';

@NgModule({
  imports: [
    CommonModule,
    ConsultaRoutingModule,
    SharedModule,
    NgxPaginationModule,
    TableModule,
    DropdownModule,
    MultiSelectModule,
    SliderModule,
    ButtonModule,
    CalendarModule,
    ToggleButtonModule,
    SpinnerModule,
    SelectButtonModule,
    ScrollPanelModule,
    OverlayPanelModule
  ],
    declarations: [
      MisConsultasComponent,
      CrearConsultaComponent,
      DetalleConsultaComponent
    ],
    providers: [
      ConsultaService
      /*
      LiquidacionAprobadaService,
      LiquidacionObservadaService,
      LiquidacionPagaService,
      LiquidacionProformaService, */
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ConsultaModule { }
