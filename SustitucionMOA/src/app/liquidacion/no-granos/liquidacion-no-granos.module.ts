import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { LiquidacionNGAprobadaComponent } from "./aprobada/liquidacion.no-granos.aprobada.component";
import { LiquidacionNGObservadaComponent } from "./observada/liquidacion.no-granos.observada.component";
import { LiquidacionNoGranosRoutingModule } from './liquidacion-no-granos-routing.module';
import { LiquidacionNGAprobadaService, LiquidacionNGObservadaService, LiquidacionNGPagaService, LiquidacionService } from '../liquidacion.service';

@NgModule({
  imports: [
    CommonModule,
    LiquidacionNoGranosRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      LiquidacionNGAprobadaComponent,
      LiquidacionNGObservadaComponent
    ],
    providers: [
      LiquidacionService,
      LiquidacionNGAprobadaService,
      LiquidacionNGObservadaService,
      LiquidacionNGPagaService,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LiquidacionNoGranosModule { }
