import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { CuentaCorrienteRoutingModule } from './cuenta-corriente-routing.module';
import { CuentaCorrienteService, CuentaCorrienteAgrupadaService, CuentaCorrientePartidasAbiertasService } from "./cuenta-corriente.service";
import { CuentaCorrienteAgrupadaComponent } from './agrupada/cuenta-corriente.agrupada.component';
import { CuentaCorrientePartidasAbiertasComponent } from './partidas-abiertas/cuenta-corriente.partidas-abiertas.component';
import { CuentaCorrienteBaseComponent } from './cuenta-corriente.component';

@NgModule({
    imports: [
        CommonModule,
        CuentaCorrienteRoutingModule,
        SharedModule,
        NgxPaginationModule
    ],
    declarations: [
        CuentaCorrienteBaseComponent,
        CuentaCorrienteAgrupadaComponent,
        CuentaCorrientePartidasAbiertasComponent
    ],
    providers: [
        CuentaCorrienteService,
        CuentaCorrienteAgrupadaService,
        CuentaCorrientePartidasAbiertasService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class CuentaCorrienteModule { }
