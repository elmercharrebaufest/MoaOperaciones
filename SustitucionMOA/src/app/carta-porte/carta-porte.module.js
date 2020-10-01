var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedModule } from '../common/shared.module';
import { CartaPorteAplicacionComponent } from "./aplicacion/carta-porte.aplicacion2.component";
import { CartaPorteRoutingModule } from './carta-porte-routing.module';
import { CartaPorteBaseComponent } from "./carta-porte.component";
import { CartaPorteService } from './carta-porte2.service';
import { CartaPorteDescargaComponent } from "./descarga/carta-porte.descarga2.component";
import { CartaPorteDetalleComponent } from "./detalle/carta-porte.detalle2.component";
import { CartaPorteFormularioComponent } from "./formulario/carta-porte.formulario.component";
var CartaPorteModule = /** @class */ (function () {
    function CartaPorteModule() {
    }
    CartaPorteModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                FormsModule,
                CartaPorteRoutingModule,
                SharedModule,
                NgxPaginationModule,
                Ng2AutoCompleteModule,
                AutocompleteLibModule,
            ],
            declarations: [
                CartaPorteAplicacionComponent,
                CartaPorteBaseComponent,
                CartaPorteDescargaComponent,
                CartaPorteDetalleComponent,
                CartaPorteFormularioComponent
            ],
            providers: [
                CartaPorteService
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
        })
    ], CartaPorteModule);
    return CartaPorteModule;
}());
export { CartaPorteModule };
//# sourceMappingURL=carta-porte.module.js.map