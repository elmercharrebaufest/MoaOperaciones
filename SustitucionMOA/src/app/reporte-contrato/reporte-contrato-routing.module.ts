import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ReporteContratoListado } from './listado/reporte-contrato.listado.component';


const routes: Routes = [
    { path: '', component: ReporteContratoListado },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReporteContratoRoutingModule { }
