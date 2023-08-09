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
import { EcheqPopupComponent } from './gestion/echeq.popup/echeq-popup.component';
import { CheckboxModule } from 'primeng/checkbox';
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {ConfirmationService} from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { MisEcheqComponent } from './mis-echeq/mis-echeq.component';
import { MisEcheqFiltrosComponent } from './mis-echeq/mis-echeq.filtros/mis-echeq-filtros.component';
import { MisEcheqGrillaComponent } from './mis-echeq/mis-echeq.grilla/mis-echeq-grilla.component';
import { NgxCurrencyModule } from "ngx-currency";
import { NgxMaskModule } from "ngx-mask";
import { CustomPipeEcheq } from '../common/pipes/customPipeEcheq';


@NgModule({
    imports: [
        CommonModule,
        EcheqRoutingModule,
        SharedModule,
        NgxPaginationModule,
        ReCaptchaModule,
        CalendarModule,
        CheckboxModule,
        ConfirmDialogModule,
        DialogModule,
        TooltipModule,
        NgxCurrencyModule,
        NgxMaskModule
    ],
    declarations: [
        EcheqGestionComponent,
        FiltrosComponent,
        GrillaComponent,
        EcheqPopupComponent,
        MisEcheqComponent,
        MisEcheqFiltrosComponent,
        MisEcheqGrillaComponent,
        CustomPipeEcheq
    ],
    providers: [
        EcheqService,
        ConfirmationService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class EcheqModule { }
