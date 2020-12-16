import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ReporteContratoComponent } from './contrato/reporte.contrato.component';
import { ReporteCupoComponent } from './cupo/reporte.cupo.component';


const routes: Routes = [
    { path: '', component: ReporteContratoComponent },
    { path: 'contrato', component: ReporteContratoComponent },
    { path: "cupo", component: ReporteCupoComponent },

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReporteRoutingModule { }
