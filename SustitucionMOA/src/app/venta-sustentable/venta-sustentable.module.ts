import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { VentaSustentableService } from './venta-sustentable.service';
import { VentaSustentableRoutingModule } from './venta-sustentable-routing.Module';
import { VentaSustentableBaseComponent } from "./venta-sustentable.component";
import { ListadoCamposComponent } from "./listado-campos/listado-campos.component";
import { AltaComponent } from "./alta/alta.component";
import { EdicionComponent } from "./edicion/edicion.component";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReCaptchaModule } from 'angular2-recaptcha';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { SpinnerModule } from 'primeng/spinner';
import {AutoCompleteModule} from 'primeng/autocomplete';

@NgModule({
  imports: [
    CommonModule,
    VentaSustentableRoutingModule,
    SharedModule,
    NgxPaginationModule,
    ReCaptchaModule,
    FormsModule,
    ReactiveFormsModule,
    AutoCompleteModule, DropdownModule, MultiSelectModule, SpinnerModule
  ],
    declarations: [
      ListadoCamposComponent,
      AltaComponent,
      EdicionComponent,
    ],
    providers: [
        VentaSustentableService,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class VentaSustentableModule { }
