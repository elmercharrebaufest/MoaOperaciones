import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { SpinnerModule } from 'primeng/spinner';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { DialogModule } from 'primeng/dialog';
import { ChipsModule } from 'primeng/chips';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';

import { FiltroFechaComponent } from "./view-child/filtro-fecha/filtro-fecha.component";
import { DropdownComponent } from "./view-child/dropdown/dropdown.component";
import { MensajeComponent } from "./view-child/mensaje/mensaje.component";
import { MensajeModalComponent } from "./view-child/mensaje-modal/mensaje-modal.component";
import { SpinnerComponent } from "./view-child/spinner/spinner.component";
import { SpinnerSmallComponent } from "./view-child/spinner-small/spinner-small.component";
import { SpinnerLoadingComponent } from './view-child/spinner-loading/spinner-loading.component';
import { CustomFilter } from "./pipes/customFilter";
import { CustomFilterOr } from "./pipes/customFilterOr";
import { CustomFilterContain } from "./pipes/customFilterContain";
import { CustomNumericFilter } from "./pipes/customNumericFilter";
import { OrderedColumn } from "./pipes/orderedColumn";
import { ShortenStringPipe } from "./pipes/shortenString";
import { BaseComponent } from './base-components/base-component';
import { ListBaseComponent } from './base-components/list-base-component';
import { ArchivoPipe } from './pipes/archivos.pipe';
import { InformeComercialComponent } from '../alta-proveedores/informe-comercial/informe-comercial.component';
import { CartaPresentacionComponent } from '../alta-proveedores/carta-presentacion/carta-presentacion.component';
import { SeleccionarProveedorComponent } from './shared-components/seleccionar-proveedor/seleccionar-proveedor.component';
import { NumericDirective } from './directive/numeric.directive';
import { AutocompleteLocalidadComponent } from './shared-components/autocomplete-localidad/autocomplete-localidad.component'
import { StepperComponent } from './view-child/stepper/stepper.component';
import { WeekdaySelectComponent } from './view-child/weekday-select/weekday-select.component';
import { CustomFilterBoolean } from "./pipes/customFilterBoolean";
import { BuscadorComponent } from './shared-components/buscador/buscador.component'
import { HighlightDirective } from './directive/clickOutside.directive';
import { BuscadorSmallComponent } from './shared-components/buscador/buscador-small/buscadorSmall.component';
import { EmailComposeComponent } from './email-compose/email-compose.component'
import { EmailComposeService } from './email-compose/email-compose.service';
import { CustomDateFormat } from './pipes/jsonDate.pipe';;
import { FiltroFechaReporteComponent } from './view-child/filtro-fecha-reporte/filtro-fecha-reporte.component';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownInputComponent } from './shared-components/dropdown-input/dropdown-input.component';
import { FiltroFechaFasComponent } from './view-child/filtro-fecha-fas/filtro-fecha-fas.component';
import { CustomNumberPipe } from './pipes/customNumericFilter copy';
import { FormatNumericDirective } from './directive/format.numeric.directive';
import { CustomTipoProveedorFilter } from './pipes/customTipoProveedorFilter';
import { CustomFilterEstados } from './pipes/customFilterEstados';

import { CustomDefaultDataPipe } from './pipes/customDefaultData.pipe'

const declaredAndExported = [
    FiltroFechaComponent,
    DropdownComponent,
    MensajeComponent,
    MensajeModalComponent,
    SpinnerComponent,
    SpinnerSmallComponent,
    SpinnerLoadingComponent,
    CustomFilter,
    CustomFilterOr,
    CustomFilterContain,
    CustomNumericFilter,
    OrderedColumn,
    ShortenStringPipe,
    BaseComponent,
    ListBaseComponent,
    ArchivoPipe,
    AutocompleteLocalidadComponent,
    SeleccionarProveedorComponent,
    InformeComercialComponent,
    CartaPresentacionComponent,
    NumericDirective,
    StepperComponent,
    WeekdaySelectComponent,
    BuscadorSmallComponent,
    CustomFilterBoolean,
    BuscadorComponent,
    EmailComposeComponent,
    CustomDateFormat,
    FiltroFechaReporteComponent,
    FiltroFechaFasComponent,
    DropdownInputComponent,
    CustomNumberPipe,
    FormatNumericDirective,
    CustomTipoProveedorFilter,
    CustomFilterEstados
    ,
    CustomDefaultDataPipe
];
@NgModule({
    imports: [CommonModule,
        FormsModule,
        Ng2AutoCompleteModule,
        AutocompleteLibModule,
        DropdownModule,
        MultiSelectModule,
        SpinnerModule,
        AutoCompleteModule,
        OverlayPanelModule,
        ToggleButtonModule,
        ReactiveFormsModule,
        DialogModule,
        ChipsModule,
        InputTextModule,
        InputTextareaModule,
        CheckboxModule
    ],
    exports: [
        CommonModule,
        FormsModule,
        Ng2AutoCompleteModule,
        AutocompleteLibModule,
        ...declaredAndExported
    ],
    declarations: [
        HighlightDirective,
        ...declaredAndExported,
        CustomDefaultDataPipe
    ],
    providers: [
        EmailComposeService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class SharedModule { }
