import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { FacturaService } from './factura.service';
import { FacturaRoutingModule } from './factura-routing.module';
import { FacturaComponent } from './factura.component';
import { FileUploadModule } from 'primeng/fileupload';

@NgModule({
  imports: [
    CommonModule,
    FacturaRoutingModule,
    SharedModule,
        NgxPaginationModule,
        FileUploadModule
  ],
    declarations: [
      FacturaComponent
    ],
    providers: [
        FacturaService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class FacturaModule { }
