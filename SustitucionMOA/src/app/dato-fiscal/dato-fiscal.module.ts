import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { DatoFiscalRoutingModule } from './dato-fiscal-routing.module';
import { DatoFiscalService } from './dato-fiscal.service';
import { DatoFiscalBaseComponent } from './dato-fiscal.component';
import { VendedoresListComponent } from './vendedor/dato-fiscal.vendedor.component';
import { DocumentacionComponent } from './documentacion/documentacion.component';

@NgModule({
  imports: [
    CommonModule,
    DatoFiscalRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      DatoFiscalBaseComponent,
      VendedoresListComponent,
      DocumentacionComponent
    ],
    providers: [
        DatoFiscalService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DatoFiscalModule { }
