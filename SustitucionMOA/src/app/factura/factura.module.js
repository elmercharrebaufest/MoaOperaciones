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
import { FacturaService } from './factura.service';
import { FacturaRoutingModule } from './factura-routing.module';
import { FacturaComponent } from './factura.component';
var FacturaModule = /** @class */ (function () {
    function FacturaModule() {
    }
    FacturaModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                FacturaRoutingModule,
                SharedModule,
                NgxPaginationModule
            ],
            declarations: [
                FacturaComponent
            ],
            providers: [
                FacturaService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], FacturaModule);
    return FacturaModule;
}());
export { FacturaModule };
//# sourceMappingURL=factura.module.js.map