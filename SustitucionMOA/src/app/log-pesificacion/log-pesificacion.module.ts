import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { TableModule } from 'primeng/table';
import { SpinnerModule } from 'primeng/spinner';
import { ListadoLogPesificacionComponent } from './listado-log-pesificacion/listado-log-pesificacion.component';
import { ListadoLogPesificacionMasivoComponent } from './listado-masivo/listado-log-pesificacion-masivo.component';
import { LogPesificacionService } from './log-pesificacion.service';
import { LogPesificacionRoutingModule } from './log-pesificacion-routing.module';
import { LogPesificacionTab } from './log-pesificacion-tabs.component';


@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        NgxPaginationModule,
        TableModule,
        SpinnerModule,
        LogPesificacionRoutingModule
    ],
    exports:[
        ListadoLogPesificacionComponent,
        LogPesificacionTab,
        ListadoLogPesificacionMasivoComponent],
    declarations: [
        ListadoLogPesificacionComponent,
        LogPesificacionTab,
        ListadoLogPesificacionMasivoComponent
    ],
    providers: [
        LogPesificacionService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LogPesificacionModule { }