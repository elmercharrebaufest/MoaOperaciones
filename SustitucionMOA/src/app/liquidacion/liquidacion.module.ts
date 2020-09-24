import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { LiquidacionAprobadaComponent } from "./aprobada/liquidacion.aprobada.component";
import { LiquidacionObservadaComponent } from "./observada/liquidacion.observada.component";
import { LiquidacionPagaComponent } from "./paga/liquidacion.paga.component";
import { LiquidacionProformaComponent } from "./proforma/liquidacion.proforma.component";
import { LiquidacionAprobadaService, LiquidacionObservadaService, LiquidacionPagaService, LiquidacionProformaService, LiquidacionService } from './liquidacion.service';
import { LiquidacionRoutingModule } from './liquidacion-routing.module';

@NgModule({
  imports: [
    CommonModule,
    LiquidacionRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      LiquidacionAprobadaComponent,
      LiquidacionObservadaComponent,
      LiquidacionPagaComponent,
      LiquidacionProformaComponent
    ],
    providers: [
      LiquidacionService,
      LiquidacionAprobadaService,
      LiquidacionObservadaService,
      LiquidacionPagaService,
      LiquidacionProformaService,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LiquidacionModule { }
