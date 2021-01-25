import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { OrdenesDeCargaRoutingModule } from './ordenes-de-carga-routing.module';
import { OrdenesDeCargaAlta } from './alta/ordenes-de-carga.alta.component';
import { OrdenesDeCargaListado } from './listado/ordenes-de-carga.listado.component';
import { OrdenesDeCargaService } from './ordenes-de-carga.service';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        NgxPaginationModule,
        OrdenesDeCargaRoutingModule
    ],
    declarations: [
        OrdenesDeCargaAlta,
        OrdenesDeCargaListado
    ],
    providers: [
        OrdenesDeCargaService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class OrdenesDeCargaModule { }
