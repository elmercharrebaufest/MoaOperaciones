import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { OrdenesDeCargaRoutingModule } from './ordenes-de-carga-routing.module';
import { OrdenesDeCargaAlta } from './alta/ordenes-de-carga.alta.component';
import { OrdenesDeCargaListado } from './listado/ordenes-de-carga.listado.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NgxMaskModule } from "ngx-mask";
import { OrdenesDeCargaService } from './ordenes-de-carga.service';
import { OrdenesDeCargaDetalleComponent } from './detalle/ordenes-de-carga.detalle.component';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogModule } from 'primeng/dialog';
import { CheckboxModule } from 'primeng/checkbox';
import { TooltipModule } from 'primeng/tooltip';
import { MessageModule } from 'primeng/message';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        NgxPaginationModule,
        ButtonModule,
        DropdownModule,
        AutoCompleteModule,
        NgxSpinnerModule,
        NgxMaskModule,
        OrdenesDeCargaRoutingModule,
        MultiSelectModule,
        ConfirmDialogModule,
        ProgressSpinnerModule,
        DialogModule,
        CheckboxModule,
        TooltipModule,
        MessageModule,
    ],
    declarations: [
        OrdenesDeCargaAlta,
        OrdenesDeCargaListado,
        OrdenesDeCargaDetalleComponent
    ],
    providers: [
        OrdenesDeCargaService
    ],
    exports:[
        OrdenesDeCargaAlta,
        OrdenesDeCargaListado,
        OrdenesDeCargaDetalleComponent
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class OrdenesDeCargaModule { }
