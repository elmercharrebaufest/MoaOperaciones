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
import { TooltipModule } from 'primeng/tooltip';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    CartaPorteRoutingModule,
    SharedModule,
    NgxPaginationModule,
    Ng2AutoCompleteModule,
    AutocompleteLibModule,
    TooltipModule
  ],
  declarations: [
    CartaPorteAplicacionComponent,
    CartaPorteBaseComponent,
    CartaPorteDescargaComponent,
    CartaPorteDetalleComponent,
    CartaPorteFormularioComponent
  ],
  providers: [
    CartaPorteService,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class CartaPorteModule { }
