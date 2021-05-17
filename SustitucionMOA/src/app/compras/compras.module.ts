import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { SolpService } from './solp.service';
import { Generacion2Component } from './PliegoPasos/generacion2.component';
import { Generacion1Component } from './PliegoPasos/generacion1.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { SolpComponent } from './solp.component';
import { FormsModule } from '@angular/forms';
import { ComprasRoutingModule } from './compras-routing.module';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { EditorModule } from 'primeng/editor';
import { PliegoPreviewComponent } from './preview/pliego.preview.component';
import { SidebarModule } from 'primeng/sidebar';
import { CheckboxModule } from 'primeng/checkbox';
import { DragAndDropDirective } from '../directivas/drag-and-drop.directive';
import { EspecificacionesComponent } from './PliegoPasos/solapaTres/especificaciones.component'
import {FileUploadModule} from 'primeng/fileupload';
import { QuillModule } from 'ngx-quill'


@NgModule({
    imports: [
        CommonModule,
        ComprasRoutingModule,
        FormsModule,
        SharedModule,
        InputTextModule,
        CalendarModule,
        DialogModule,
        EditorModule,
        SidebarModule,
        CheckboxModule,
        FileUploadModule,
        QuillModule.forRoot()
    ],
    declarations: [
        SolpComponent,
        DashboardComponent,
        Generacion1Component,
        Generacion2Component,
        PliegoPreviewComponent,
        DragAndDropDirective,
        EspecificacionesComponent
    ],
    providers: [
        SolpService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ComprasModule { }
