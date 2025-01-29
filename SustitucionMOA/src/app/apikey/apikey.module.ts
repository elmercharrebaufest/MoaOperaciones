import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { ApikeyRoutingModule } from './apikey-routing.module';
import { ApikeyComponent } from './apikey.component';
import { ApikeyService } from './apikey.service';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        ApikeyRoutingModule
    ],
    declarations: [
        ApikeyComponent,
        
    ],
    exports: [
       
    ],
    providers: [
        ApikeyService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ApikeyModule { }

