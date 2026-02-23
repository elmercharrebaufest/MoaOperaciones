import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ConsultaTicketPesadaRoutingModule } from './consulta-ticket-pesada-routing.module';
import { ConsultaTicketPesadaComponent } from './consulta-ticket-pesada.component';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { NgxMaskModule } from 'ngx-mask';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    ConsultaTicketPesadaRoutingModule,
    NgxPaginationModule,
    NgxMaskModule.forRoot()
  ],
  declarations: [ConsultaTicketPesadaComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  
})
export class ConsultaTicketPesadaModule { }
