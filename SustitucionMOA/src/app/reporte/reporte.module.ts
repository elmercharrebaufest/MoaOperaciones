import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReporteRoutingModule } from './reporte-routing.module';
import { ReporteBaseComponent } from './reporte.component';
import { ReporteContratoComponent } from './contrato/reporte.contrato.component';
import { ReporteCupoComponent } from './cupo/reporte.cupo.component';
import { ReporteService } from './reporte.service';
import { SpinnerSmallComponent } from '../common/view-child/spinner-small/spinner-small.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';

@NgModule({
  imports: [
    CommonModule,
    ReporteRoutingModule,
    SharedModule,
    NgxPaginationModule,
    AutocompleteLibModule
  ],
    declarations: [
      ReporteBaseComponent,
      ReporteContratoComponent,
      ReporteCupoComponent,
    ],
    providers: [
        ReporteService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ReporteModule { }
