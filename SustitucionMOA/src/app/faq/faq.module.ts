import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedModule } from '../common/shared.module';
import { FaqRoutingModule } from './faq-routing.module';
import { FaqComponent } from './faq.component';
import { FaqService } from './faq.service';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FaqRoutingModule,
        NgxPaginationModule        
    ],
    declarations: [
        FaqComponent,
        
    ],
    exports: [
       
    ],
    providers: [
        FaqService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class FaqModule { }

