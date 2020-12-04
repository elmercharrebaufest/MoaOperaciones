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
import { FleteAFacturarComponent } from "./a-facturar/flete.a-facturar.component";
import { FleteFacturadoComponent } from "./facturado/flete.facturado.component";
import { FletePendienteComponent } from "./pendiente/flete.pendiente.component";
import { FleteRoutingModule } from './flete-routing.module';
import { FleteService, FleteAFacturarService, FleteFacturadoService, FletePendienteService } from './flete.service';
var FleteModule = /** @class */ (function () {
    function FleteModule() {
    }
    FleteModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                FleteRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                FleteAFacturarComponent,
                FleteFacturadoComponent,
                FletePendienteComponent
            ],
            providers: [
                FleteService,
                FleteAFacturarService,
                FleteFacturadoService,
                FletePendienteService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], FleteModule);
    return FleteModule;
}());
export { FleteModule };
//# sourceMappingURL=flete.module.js.map