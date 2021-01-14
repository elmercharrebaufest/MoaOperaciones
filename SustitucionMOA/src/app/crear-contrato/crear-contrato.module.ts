import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrearContratoRoutingModule } from './crear-contrato-routing.module';
import { CrearContratoBaseComponent } from './crear-contrato.component';
import { CrearContratoAPrecioComponent } from './aprecio/crear-contrato.aprecio.component';
import { CrearContratoAFijarComponent } from './afijar/crear-contrato.afijar.component';
import { CrearContratoFijacionComponent } from './fijacion/crear-contrato.fijacion.component';
import { CrearContratoAltaMasivaComponent } from './alta-masiva/crear-contrato.alta-masiva.component';
import { CrearContratoService } from './crear-contrato.service';
import { SpinnerSmallComponent } from '../common/view-child/spinner-small/spinner-small.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';

@NgModule({
    imports: [
        CommonModule,
        CrearContratoRoutingModule,
        SharedModule,
        NgxPaginationModule,
        AutocompleteLibModule
    ],
    declarations: [
        CrearContratoBaseComponent,
        CrearContratoFijacionComponent,
        CrearContratoAFijarComponent,
        CrearContratoAPrecioComponent,
        CrearContratoAltaMasivaComponent,
    ],
    providers: [
        CrearContratoService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class CrearContratoModule { }
