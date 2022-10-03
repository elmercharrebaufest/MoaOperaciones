import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';

import { MisEcheqComponent } from './mis-echeq/mis-echeq.component';
import { GestionEcheqComponent } from './gestion/gestion.component';
import { EcheqRoutingModule } from './echeq-routing.module';
import { EcheqService } from './echeq.service';
import { EcheqComponent } from './echeq.component';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        EcheqRoutingModule
   
    ],
    declarations: [     
        EcheqComponent,
        MisEcheqComponent,
        GestionEcheqComponent
    ],
    providers: [
        EcheqService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA]
})
export class EcheqModule { }
