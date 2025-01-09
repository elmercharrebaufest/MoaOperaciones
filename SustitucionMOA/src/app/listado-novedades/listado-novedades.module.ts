import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListadoNovedadesService } from './listado-novedades.service';
import { ListadoNovedadesComponent } from './listado-novedades.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { ListadoNovedadesRoutingModule } from './listado-novedades-routing.module';
import { SharedModule } from '../common/shared.module';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        ListadoNovedadesRoutingModule,
        NgxPaginationModule        
    ],
    declarations: [
        ListadoNovedadesComponent,
        
    ],
    exports: [
       
    ],
    providers: [
        ListadoNovedadesService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ListadoNovedadesModule { }

