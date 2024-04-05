import { CommonModule } from "@angular/common";
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from "@angular/core";
import { NgxMaskModule } from "ngx-mask";
import { NgxPaginationModule } from "ngx-pagination";
import { NgxSpinnerModule } from "ngx-spinner";
import { MessageService } from "primeng/api";
import { AutoCompleteModule } from "primeng/autocomplete";
import { ButtonModule } from "primeng/button";
import { CalendarModule } from "primeng/calendar";
import { CheckboxModule } from "primeng/checkbox";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { DialogModule } from "primeng/dialog";
import { DropdownModule } from "primeng/dropdown";
import { MultiSelectModule } from "primeng/multiselect";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { ToastModule } from "primeng/toast";
import { TooltipModule } from "primeng/tooltip";
import { SharedModule } from "../common/shared.module";
import { OrdenesResiduosRoutingModule } from "./ordenes-residuos-routing.module";
import { OrdenesResiduosService } from "./ordenes-residuos.service";
import { OrdenesResiduosListadoComponent } from "./listado/ordenes-residuos.listado.component";

@NgModule({
    imports: [
        CommonModule,
        OrdenesResiduosRoutingModule,
        SharedModule,
        NgxPaginationModule,
        ButtonModule,
        DropdownModule,
        AutoCompleteModule,
        NgxSpinnerModule,
        NgxMaskModule,
        MultiSelectModule,
        ConfirmDialogModule,
        CalendarModule,
        CheckboxModule,
        ProgressSpinnerModule,
        DialogModule,
        TooltipModule,
        ToastModule
    ],
    declarations: [
        OrdenesResiduosListadoComponent
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [
        OrdenesResiduosService,
        MessageService
    ]
})

export class OrdenesResiduosModule { }