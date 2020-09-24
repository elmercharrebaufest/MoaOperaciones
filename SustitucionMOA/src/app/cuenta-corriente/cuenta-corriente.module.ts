import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { CuentaCorrienteRoutingModule } from './cuenta-corriente-routing.module';
import { CuentaCorrienteService, CuentaCorrienteAgrupadaService, } from "./cuenta-corriente.service";
import { CuentaCorrienteAgrupadaComponent } from './agrupada/cuenta-corriente.agrupada.component';
import { CuentaCorrienteBaseComponent } from './cuenta-corriente.component';

@NgModule({
  imports: [
    CommonModule,
    CuentaCorrienteRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      CuentaCorrienteBaseComponent,
      CuentaCorrienteAgrupadaComponent
    ],
    providers: [
        CuentaCorrienteService,
        CuentaCorrienteAgrupadaService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class CuentaCorrienteModule { }
