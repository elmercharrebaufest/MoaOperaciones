var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
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
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { CrearContratoAFijarComponent } from '../crear-contrato/afijar/crear-contrato.afijar.component';
import { CrearContratoAPrecioComponent } from '../crear-contrato/aprecio/crear-contrato.aprecio.component';
import { CrearContratoBaseComponent } from '../crear-contrato/crear-contrato.component';
var ContratoModule = /** @class */ (function () {
    function ContratoModule() {
    }
    ContratoModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                ContratoRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                ContratoBaseComponent,
                ContratoAmpliacionComponent,
                ContratoAnulacionComponent,
                ContratoFijacionComponent,
                ContratoVigenteComponent,
                ContratoDetalleComponent,
                ContratoDetalleFijacionComponent,
                CrearContratoBaseComponent,
                CrearContratoAPrecioComponent,
                CrearContratoAFijarComponent
            ],
            providers: [
                ContratoService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], ContratoModule);
    return ContratoModule;
}());
export { ContratoModule };
//# sourceMappingURL=contrato.module.js.map