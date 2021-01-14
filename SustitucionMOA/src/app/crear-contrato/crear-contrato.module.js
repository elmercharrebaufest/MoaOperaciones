var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrearContratoRoutingModule } from './crear-contrato-routing.module';
import { CrearContratoBaseComponent } from './crear-contrato.component';
import { CrearContratoAPrecioComponent } from './aprecio/crear-contrato.aprecio.component';
import { CrearContratoAFijarComponent } from './afijar/crear-contrato.afijar.component';
import { CrearContratoFijacionComponent } from './fijacion/crear-contrato.fijacion.component';
import { CrearContratoAltaMasivaComponent } from './alta-masiva/crear-contrato.alta-masiva.component';
import { CrearContratoService } from './crear-contrato.service';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';
var CrearContratoModule = /** @class */ (function () {
    function CrearContratoModule() {
    }
    CrearContratoModule = __decorate([
        NgModule({
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
    ], CrearContratoModule);
    return CrearContratoModule;
}());
export { CrearContratoModule };
//# sourceMappingURL=crear-contrato.module.js.map