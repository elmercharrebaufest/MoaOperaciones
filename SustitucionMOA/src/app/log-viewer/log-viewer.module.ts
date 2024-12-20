import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedModule } from '../common/shared.module';
import { LogViewerComponent } from './log-viewer.component';
import { LogViewerService } from './log-viewer.service';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        LogViewerComponent,
        NgxPaginationModule        
    ],
    declarations: [
        LogViewerComponent,
        
    ],
    exports: [
       
    ],
    providers: [
        LogViewerService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LogViewerModule { }

