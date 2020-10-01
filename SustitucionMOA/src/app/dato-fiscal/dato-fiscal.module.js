var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { DatoFiscalRoutingModule } from './dato-fiscal-routing.module';
import { DatoFiscalService } from './dato-fiscal.service';
import { DatoFiscalBaseComponent } from './dato-fiscal.component';
import { VendedoresListComponent } from './vendedor/dato-fiscal.vendedor.component';
import { DocumentacionComponent } from './documentacion/documentacion.component';
import { VendedoresPendientesComponent } from './vendedores-pendientes/vendedores-pendientes.component';
var DatoFiscalModule = /** @class */ (function () {
    function DatoFiscalModule() {
    }
    DatoFiscalModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                DatoFiscalRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                DatoFiscalBaseComponent,
                VendedoresListComponent,
                DocumentacionComponent,
                VendedoresPendientesComponent
            ],
            providers: [
                DatoFiscalService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], DatoFiscalModule);
    return DatoFiscalModule;
}());
export { DatoFiscalModule };
//# sourceMappingURL=dato-fiscal.module.js.map