import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { OrdenesDeCargaRoutingModule } from './ordenes-de-carga-routing.module';
import { OrdenesDeCargaAlta } from './alta/ordenes-de-carga.alta.component';
import { OrdenesDeCargaListado } from './listado/ordenes-de-carga.listado.component';
import { NgxMaskModule } from "ngx-mask";
import { OrdenesDeCargaService } from './ordenes-de-carga.service';
import { OrdenesDeCargaDetalleComponent } from './detalle/ordenes-de-carga.detalle.component';
import { ButtonModule } from 'primeng/button';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        NgxPaginationModule,
        ButtonModule,
        NgxMaskModule,
        OrdenesDeCargaRoutingModule
    ],
    declarations: [
        OrdenesDeCargaAlta,
        OrdenesDeCargaListado,
        OrdenesDeCargaDetalleComponent
    ],
    providers: [
        OrdenesDeCargaService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class OrdenesDeCargaModule { }
