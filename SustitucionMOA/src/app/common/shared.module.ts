import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { FiltroFechaComponent } from "./view-child/filtro-fecha/filtro-fecha.component";
import { DropdownComponent } from "./view-child/dropdown/dropdown.component";
import { MensajeComponent } from "./view-child/mensaje/mensaje.component";
import { MensajeModalComponent } from "./view-child/mensaje-modal/mensaje-modal.component";
import { SpinnerComponent } from "./view-child/spinner/spinner.component";
import { SpinnerSmallComponent } from "./view-child/spinner-small/spinner-small.component";

import { CustomFilter } from "./pipes/customFilter";
import { CustomFilterOr } from "./pipes/customFilterOr";
import { CustomFilterContain } from "./pipes/customFilterContain";
import { OrderedColumn } from "./pipes/orderedColumn";
import { ShortenStringPipe } from "./pipes/shortenString";
import { BaseComponent } from './base-components/base-component';
import { ListBaseComponent } from './base-components/list-base-component';

@NgModule({
    imports: [CommonModule, FormsModule],
    declarations: [FiltroFechaComponent, DropdownComponent, MensajeComponent, MensajeModalComponent, SpinnerComponent, SpinnerSmallComponent, CustomFilter, CustomFilterOr, CustomFilterContain, OrderedColumn, ShortenStringPipe, BaseComponent, ListBaseComponent],
    exports: [FiltroFechaComponent, DropdownComponent, MensajeComponent, MensajeModalComponent, SpinnerComponent, SpinnerSmallComponent, CustomFilter, CustomFilterOr, CustomFilterContain, OrderedColumn, ShortenStringPipe,
        CommonModule, FormsModule]
})
export class SharedModule { }