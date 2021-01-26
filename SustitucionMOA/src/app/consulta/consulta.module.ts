import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
//import { LiquidacionAprobadaService, LiquidacionObservadaService, LiquidacionPagaService, LiquidacionProformaService, LiquidacionService } from './liquidacion.service';
import { ConsultaRoutingModule } from './consulta-routing.module';
import { ConsultaService } from './consulta.service';
import { CrearConsultaComponent } from './crear-consulta/crear-consulta.component';
import { MisConsultasComponent } from './mis-consultas/mis-consultas.component';

@NgModule({
  imports: [
    CommonModule,
    ConsultaRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      MisConsultasComponent,
      CrearConsultaComponent
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
