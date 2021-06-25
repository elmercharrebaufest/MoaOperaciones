import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { ComprasService } from './compras.service';
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
import { CotizacionComponent } from './PliegoPasos/cotizacion.component';
import { DragAndDropDirective } from '../directivas/drag-and-drop.directive';
import { KeyFilterModule } from 'primeng/keyfilter';
import { EspecificacionesComponent } from './PliegoPasos/solapaTres/especificaciones.component'
import { FileUploadModule } from 'primeng/fileupload';
import { QuillModule } from 'ngx-quill'
import { CabeceraComponent } from './SolpPasos/cabecera.component';
import { RadioButtonModule } from 'primeng/radiobutton';
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ReactiveFormsModule } from '@angular/forms';
import { ValidadorPasoSolpService } from './validadorPasoSolpService';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { ChipsModule } from 'primeng/chips';
import { TableModule } from 'primeng/table';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SubPosicionComponent } from './PliegoPasos/solapaSubposiciones/subPosicion.component';
import {ConfirmationService} from 'primeng/api';

    
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
        KeyFilterModule,
        FileUploadModule,
        QuillModule.forRoot(),
        MessagesModule,
        MessageModule,
        ToastModule,
        RadioButtonModule,
        DropdownModule,
        AutoCompleteModule,
        ReactiveFormsModule,
        ChipsModule,
        TableModule,
        ConfirmDialogModule
    ],
    declarations: [
        SolpComponent,
        DashboardComponent,
        Generacion1Component,
        Generacion2Component,
        PliegoPreviewComponent,
        EspecificacionesComponent,
        CotizacionComponent,
        DragAndDropDirective,
        CabeceraComponent,
        SubPosicionComponent
    ],
    providers: [
        ComprasService,
        ValidadorPasoSolpService,
        ConfirmationService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ComprasModule { }
