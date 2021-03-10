import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { VentaSustentableService } from './venta-sustentable.service';
import { VentaSustentableRoutingModule } from './venta-sustentable-routing.Module';
/*
import { LiquidacionInformarComponent } from './informar/liquidacion.informar.component';
import { LiquidacionInformadaComponent } from './informada/liquidacion.informada.component';
*/
import { ReCaptchaModule } from 'angular2-recaptcha';

@NgModule({
  imports: [
    CommonModule,
    VentaSustentableRoutingModule,
    SharedModule,
    NgxPaginationModule,
    ReCaptchaModule
  ],
    declarations: [
        /*
      LiquidacionInformarComponent,
      LiquidacionInformadaComponent
        */
    ],
    providers: [
        VentaSustentableService,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class VentaSustentableModule { }
