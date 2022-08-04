import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReporteContratoListado } from './listado/reporte-contrato.listado.component';
import { ReporteContratoRoutingModule } from './reporte-contrato-routing.module';
import { ReporteContratoService } from './reporte-contrato.service';
import { SharedModule } from '../common/shared.module';

@NgModule({
    imports: [
        SharedModule,
        CommonModule,
        ReporteContratoRoutingModule
  ],
    declarations: [
        ReporteContratoListado

    ],
    providers :[
      ReporteContratoService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ReporteContratoModule { }
