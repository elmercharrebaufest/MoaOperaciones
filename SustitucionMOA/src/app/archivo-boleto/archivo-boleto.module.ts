import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { DialogModule } from 'primeng/dialog';
import { SharedModule } from '../common/shared.module';
import { TooltipModule } from 'primeng/tooltip';
import { ArchivoBoletoService } from './archivo-boleto.service';
import { ListarArchivoBoletoComponent } from './listar-archivo-boleto/listar-archivo-boleto.component';
import { ArchivoBoletoRoutingModule } from './archivo-boleto-routing.module';
import { ButtonModule } from 'primeng/button';
import { MultiSelectModule } from 'primeng/multiselect';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    ArchivoBoletoRoutingModule,
    NgxPaginationModule,
    TooltipModule,
    DialogModule,
    ButtonModule,
    MultiSelectModule
  ],
  declarations: [
    ListarArchivoBoletoComponent
  ],
  providers: [
    ArchivoBoletoService
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ArchivoBoletoModule { }
