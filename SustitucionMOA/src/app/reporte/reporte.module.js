var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReporteRoutingModule } from './reporte-routing.module';
import { ReporteBaseComponent } from './reporte.component';
import { ReporteContratoComponent } from './contrato/reporte.contrato.component';
import { ReporteCupoComponent } from './cupo/reporte.cupo.component';
import { ReporteService } from './reporte.service';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';
var ReporteModule = /** @class */ (function () {
    function ReporteModule() {
    }
    ReporteModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                ReporteRoutingModule,
                SharedModule,
                NgxPaginationModule,
                AutocompleteLibModule
            ],
            declarations: [
                ReporteBaseComponent,
                ReporteContratoComponent,
                ReporteCupoComponent,
            ],
            providers: [
                ReporteService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], ReporteModule);
    return ReporteModule;
}());
export { ReporteModule };
//# sourceMappingURL=reporte.module.js.map