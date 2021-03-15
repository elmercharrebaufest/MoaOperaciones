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
import { CustomNumericFilter } from "./pipes/customNumericFilter";
import { OrderedColumn } from "./pipes/orderedColumn";
import { ShortenStringPipe } from "./pipes/shortenString";
import { BaseComponent } from './base-components/base-component';
import { ListBaseComponent } from './base-components/list-base-component';
import { ArchivoPipe } from './pipes/archivos.pipe';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { SpinnerModule } from 'primeng/spinner';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { AutocompleteLocalidadComponent } from './shared-components/autocomplete-localidad/autocomplete-localidad.component'

@NgModule({
    imports: [CommonModule, FormsModule, Ng2AutoCompleteModule,
        AutocompleteLibModule,
        DropdownModule,
        MultiSelectModule,
        SpinnerModule,
        AutoCompleteModule],
    declarations: [FiltroFechaComponent, DropdownComponent, MensajeComponent, MensajeModalComponent, SpinnerComponent, SpinnerSmallComponent, CustomFilter, CustomFilterOr, CustomFilterContain, CustomNumericFilter, OrderedColumn, ShortenStringPipe, BaseComponent, ListBaseComponent, ArchivoPipe,
        AutocompleteLocalidadComponent,],
    exports: [FiltroFechaComponent, DropdownComponent, MensajeComponent, MensajeModalComponent, SpinnerComponent, SpinnerSmallComponent, CustomFilter, CustomFilterOr, CustomFilterContain, CustomNumericFilter, OrderedColumn, ShortenStringPipe, BaseComponent, ListBaseComponent, ArchivoPipe,
        CommonModule, FormsModule, AutocompleteLocalidadComponent]
})
export class SharedModule { }