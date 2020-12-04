import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { FleteAFacturarComponent } from "./a-facturar/flete.a-facturar.component";
import { FleteFacturadoComponent } from "./facturado/flete.facturado.component";
import { FletePendienteComponent } from "./pendiente/flete.pendiente.component";
import { FleteRoutingModule } from './flete-routing.module';
import { FleteService, FleteAFacturarService, FleteFacturadoService, FletePendienteService } from './flete.service';

@NgModule({
  imports: [
    CommonModule,
    FleteRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
    declarations: [
      FleteAFacturarComponent,
      FleteFacturadoComponent,
      FletePendienteComponent
    ],
    providers: [
      FleteService,
      FleteAFacturarService,
      FleteFacturadoService,
      FletePendienteService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class FleteModule { }
