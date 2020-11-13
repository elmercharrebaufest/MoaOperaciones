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
import { CuentaCorrienteRoutingModule } from './cuenta-corriente-routing.module';
import { CuentaCorrienteService, CuentaCorrienteAgrupadaService, CuentaCorrientePartidasAbiertasService } from "./cuenta-corriente.service";
import { CuentaCorrienteAgrupadaComponent } from './agrupada/cuenta-corriente.agrupada.component';
import { CuentaCorrientePartidasAbiertasComponent } from './partidas-abiertas/cuenta-corriente.partidas-abiertas.component';
import { CuentaCorrienteBaseComponent } from './cuenta-corriente.component';
var CuentaCorrienteModule = /** @class */ (function () {
    function CuentaCorrienteModule() {
    }
    CuentaCorrienteModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                CuentaCorrienteRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                CuentaCorrienteBaseComponent,
                CuentaCorrienteAgrupadaComponent,
                CuentaCorrientePartidasAbiertasComponent
            ],
            providers: [
                CuentaCorrienteService,
                CuentaCorrienteAgrupadaService,
                CuentaCorrientePartidasAbiertasService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], CuentaCorrienteModule);
    return CuentaCorrienteModule;
}());
export { CuentaCorrienteModule };
//# sourceMappingURL=cuenta-corriente.module.js.map