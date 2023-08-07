import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { ReporteContratoListado } from './listado/reporte-contrato.listado.component';
import { ReporteContratoRoutingModule } from './reporte-contrato-routing.module';
import { ReporteContratoService } from './reporte-contrato.service';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { NgxPaginationModule } from 'ngx-pagination';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { NgxMaskModule } from 'ngx-mask';
import { MultiSelectModule } from 'primeng/multiselect';
import { CheckboxModule } from 'primeng/checkbox';
import { DetalleComponent } from './detalle/reporte-contrato.detalle.component';
import { OrdenesDeCargaModule } from '../ordenes-de-carga/ordenes-de-carga.module';



@NgModule({
    imports: [
        CommonModule,
        SharedModule,      
        ReporteContratoRoutingModule,
        ConfirmDialogModule,
        NgxPaginationModule,
        ButtonModule, DropdownModule, AutoCompleteModule,
        NgxMaskModule,
        MultiSelectModule,
        CheckboxModule,
        OrdenesDeCargaModule,
  ],
    declarations: [
        ReporteContratoListado,
        DetalleComponent
       

    ],
    providers :[
        ReporteContratoService
  
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ReporteContratoModule { }
