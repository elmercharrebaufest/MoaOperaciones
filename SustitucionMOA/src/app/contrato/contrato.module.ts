import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContratoRoutingModule } from './contrato-routing.module';
import { ContratoBaseComponent } from './contrato.component';
import { ContratoAmpliacionComponent } from './ampliacion/contrato.ampliacion.component';
import { ContratoAnulacionComponent } from './anulacion/contrato.anulacion.component';
import { ContratoFijacionComponent } from './fijacion/contrato.fijacion.component';
import { ContratoVigenteComponent } from './vigente/contrato.vigente.component';
import { ContratoDetalleComponent } from './detalle/contrato.detalle.component';
import { ContratoDetalleFijacionComponent } from './detalle-fijacion/contrato.detalle-fijacion.component';
import { ContratoService } from './contrato.service';
import { SpinnerSmallComponent } from '../common/view-child/spinner-small/spinner-small.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';

@NgModule({
    imports: [
        CommonModule,
        ContratoRoutingModule,
        SharedModule,
        NgxPaginationModule,
        AutocompleteLibModule
    ],
    declarations: [
        ContratoBaseComponent,
        ContratoAmpliacionComponent,
        ContratoAnulacionComponent,
        ContratoFijacionComponent,
        ContratoVigenteComponent,
        ContratoDetalleComponent,
        ContratoDetalleFijacionComponent,
    ],
    providers: [
        ContratoService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA]
})
export class ContratoModule { }
