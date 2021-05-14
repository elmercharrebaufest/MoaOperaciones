import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListadoLogPesificacionComponent } from './listado-log-pesificacion/listado-log-pesificacion.component';
import { ListadoLogPesificacionMasivoComponent } from './listado-masivo/listado-log-pesificacion-masivo.component';
import { LogPesificacionTab } from './log-pesificacion-tabs.component';

const routes: Routes = [
    { path: '', component: LogPesificacionTab },
    { path: 'listado', component: ListadoLogPesificacionComponent },
    { path: 'listadoAutomaticas', component: ListadoLogPesificacionMasivoComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class LogPesificacionRoutingModule { }