import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { LiquidacionNoGranosRoutingModule } from './liquidacion-no-granos-routing.module';
import { LiquidacionNGRegistradoService, LiquidacionNGPagaService, LiquidacionService } from '../liquidacion.service';
import { LiquidacionNGRegistradoComponent } from './registrada/liquidacion.no-granos.registrado.component';

@NgModule({
  imports: [
    CommonModule,
    LiquidacionNoGranosRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      LiquidacionNGRegistradoComponent

    ],
    providers: [
      LiquidacionService,
      LiquidacionNGRegistradoService,
      LiquidacionNGPagaService,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LiquidacionNoGranosModule { }
