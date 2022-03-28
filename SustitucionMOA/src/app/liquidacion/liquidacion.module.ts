import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { LiquidacionAprobadaComponent } from "./aprobada/liquidacion.aprobada.component";
import { LiquidacionObservadaComponent } from "./observada/liquidacion.observada.component";
import { LiquidacionPagaComponent } from "./paga/liquidacion.paga.component";
import { LiquidacionProformaComponent } from "./proforma/liquidacion.proforma.component";
import { LiquidacionAprobadaService, LiquidacionObservadaService, LiquidacionPagaService, LiquidacionProformaService, LiquidacionService, LiquidacionInformarService, LiquidacionInformadaService } from './liquidacion.service';
import { LiquidacionRoutingModule } from './liquidacion-routing.module';
import { LiquidacionInformarComponent } from './informar/liquidacion.informar.component';
import { LiquidacionInformadaComponent } from './informada/liquidacion.informada.component';
import { ReCaptchaModule } from 'angular2-recaptcha';
import { CalendarModule } from 'primeng/calendar';

@NgModule({
  imports: [
    CommonModule,
    LiquidacionRoutingModule,
    SharedModule,
    NgxPaginationModule,
    ReCaptchaModule,
    CalendarModule
  ],
    declarations: [
      LiquidacionAprobadaComponent,
      LiquidacionObservadaComponent,
      LiquidacionPagaComponent,
      LiquidacionProformaComponent,
      LiquidacionInformarComponent,
      LiquidacionInformadaComponent
    ],
    providers: [
      LiquidacionService,
      LiquidacionAprobadaService,
      LiquidacionObservadaService,
      LiquidacionPagaService,
      LiquidacionProformaService,
      LiquidacionInformarService,
      LiquidacionInformadaService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LiquidacionModule { }
