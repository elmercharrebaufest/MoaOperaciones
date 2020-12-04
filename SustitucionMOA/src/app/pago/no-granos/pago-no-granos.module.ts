import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { PagoEmitidoNGService, PagoService } from '../pago.service';
import { PagoEmitidoNGSComponent } from './emitido/pago.no-granos.emitido.component';
import { PagoNoGranosRoutingModule } from './pago-no-granos-routing.module';

@NgModule({
  imports: [
    CommonModule,
    PagoNoGranosRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      PagoEmitidoNGSComponent,
    ],
    providers: [
        PagoService,
        PagoEmitidoNGService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class PagoNoGranosModule { }
