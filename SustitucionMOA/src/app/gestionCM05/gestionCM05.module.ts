import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { GestionCM05Component } from "./gestionCM05.component";
import { GestionCM05Service } from './gestionCM05.service';
import { GestionCM05RoutingModule } from './gestionCM05-routing.module';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { TableModule } from "primeng/table";
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { PaginatorModule } from 'primeng/paginator';
import { ToastModule } from 'primeng/toast';
import { MultiSelectModule } from 'primeng/multiselect';
import { InputMaskModule } from 'primeng/inputmask';
import { NgxMaskModule } from "ngx-mask";
import { NgxPaginationModule } from 'ngx-pagination';
import { InputTextareaModule } from 'primeng/inputtextarea';

@NgModule({
    imports: [
        GestionCM05RoutingModule,
        CommonModule,
        SharedModule,
        NgxPaginationModule,
        ConfirmDialogModule,
        TableModule,
        DialogModule,
        ButtonModule,
        PaginatorModule,
        CalendarModule,
        ToastModule,
        MultiSelectModule,
        InputMaskModule,
        NgxMaskModule,
        DropdownModule,
        InputTextareaModule,
    ],
    declarations: [
        GestionCM05Component,
    ],
    providers: [
        GestionCM05Service,
        ConfirmationService,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class GestionCM05Module { }
