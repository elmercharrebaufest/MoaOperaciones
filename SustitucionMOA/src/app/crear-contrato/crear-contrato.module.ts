import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedModule } from '../common/shared.module';
import { CrearContratoAFijarComponent } from './afijar/crear-contrato.afijar.component';
import { CrearContratoAltaMasivaComponent } from './alta-masiva/crear-contrato.alta-masiva.component';
import { CrearContratoAPrecioComponent } from './aprecio/crear-contrato.aprecio.component';
import { CrearContratoCargarNegocioComponent } from './cargarnegocio/crear-contrato.cargarnegocio.component';
import { CrearContratoRoutingModule } from './crear-contrato-routing.module';
import { CrearContratoBaseComponent } from './crear-contrato.component';
import { CrearContratoService } from './crear-contrato.service';
import { CrearContratoFijacionComponent } from './fijacion/crear-contrato.fijacion.component';

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
        CrearContratoCargarNegocioComponent,
    ],
    providers: [
        CrearContratoService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA]
})
export class CrearContratoModule { }
