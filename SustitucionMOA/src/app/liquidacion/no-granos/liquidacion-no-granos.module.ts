import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { LiquidacionNoGranosRoutingModule } from './liquidacion-no-granos-routing.module';
import { LiquidacionNGRegistradoService, LiquidacionNGPagaService, LiquidacionService, LiquidacionNGPendienteRegistroService } from '../liquidacion.service';
import { LiquidacionNGRegistradoComponent } from './registrada/liquidacion.no-granos.registrado.component';
import { LiquidacionNGPendienteRegistroComponent } from './pendiente-registro/liquidacion.no-granos.pendiente-registro.component';


@NgModule({
  imports: [
    CommonModule,
    LiquidacionNoGranosRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      LiquidacionNGRegistradoComponent,
      LiquidacionNGPendienteRegistroComponent
      

    ],
    providers: [
      LiquidacionService,
      LiquidacionNGRegistradoService,
      LiquidacionNGPagaService,
      LiquidacionNGPendienteRegistroService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LiquidacionNoGranosModule { }
