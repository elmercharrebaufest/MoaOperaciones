import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { PagoEmitidoService, PagoService } from './pago.service';
import { PagoRoutingModule } from './pago-routing.module';
import { PagoDetalleComponent } from './detalle/pago.detalle.component';
import { PagoEmitidoComponent } from './emitido/pago.emitido.component';
import { PagoComponent } from './pago.component';

@NgModule({
  imports: [
    CommonModule,
    PagoRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      PagoDetalleComponent,
      PagoEmitidoComponent,
      PagoComponent
    ],
    providers: [
        PagoService,
        PagoEmitidoService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class PagoModule { }
