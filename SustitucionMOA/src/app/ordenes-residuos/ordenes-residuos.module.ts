import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from "@angular/core";
import { CommonModule } from "@angular/common";
import { SharedModule } from "../common/shared.module";
import { NgxPaginationModule } from "ngx-pagination";
import { NgxSpinnerModule } from "ngx-spinner";
import { NgxMaskModule } from "ngx-mask";
import { ButtonModule } from "primeng/button";
import { DropdownModule } from "primeng/dropdown";
import { AutoCompleteModule } from "primeng/autocomplete";
import { MultiSelectModule } from "primeng/multiselect";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { OrdenesResiduosRoutingModule } from "./ordenes-residuos-routing.module";
import { OrdenesResiduosAltaComponent } from "./alta/ordenes-residuos.alta.component";
import { OrdenesResiduosListadoComponent } from "./listado/ordenes-residuos.listado.component";

import { OrdenesResiduosService } from "./ordenes-residuos.service";
import { CalendarModule } from "primeng/calendar";
import { CheckboxModule } from "primeng/checkbox";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { DialogModule } from "primeng/dialog";
import { TooltipModule } from "primeng/tooltip";
import { MessageService } from "primeng/api";
import { ToastModule } from "primeng/toast";
import { OrdenesResiduosDetalleComponent } from "./detalle/ordenes-residuos.detalle.component";

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
        OrdenesResiduosListadoComponent,
        OrdenesResiduosAltaComponent,
        OrdenesResiduosDetalleComponent
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [
        OrdenesResiduosService,
        MessageService
    ]
})

export class OrdenesResiduosModule { }