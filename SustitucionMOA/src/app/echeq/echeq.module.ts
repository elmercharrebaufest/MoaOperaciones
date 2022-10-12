import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { ReCaptchaModule } from 'angular2-recaptcha';
import { CalendarModule } from 'primeng/calendar';
import { EcheqRoutingModule } from './echeq-routing.module';
import { EcheqService } from './echeq.service';
import { EcheqGestionComponent } from './gestion/echeq-gestion.component';
import { GrillaComponent } from './gestion/echeq.grilla/echeq-grilla.component';
import { FiltrosComponent } from './gestion/echeq.filtros/echeq-filtros.component';
import { CheckboxModule } from 'primeng/checkbox';

@NgModule({
    imports: [
        CommonModule,
        EcheqRoutingModule,
        SharedModule,
        NgxPaginationModule,
        ReCaptchaModule,
        CalendarModule,
        CheckboxModule
    ],
    declarations: [
        EcheqGestionComponent,
        FiltrosComponent,
        GrillaComponent
    ],
    providers: [
        EcheqService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class EcheqModule { }
