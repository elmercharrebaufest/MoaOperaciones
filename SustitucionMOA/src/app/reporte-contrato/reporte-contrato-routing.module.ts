import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DetalleComponent } from './detalle/reporte-contrato.detalle.component';
import { ReporteContratoListado } from './listado/reporte-contrato.listado.component';


const routes: Routes = [
    { path: '', component: ReporteContratoListado },
    { path: "detalle/:id", component: DetalleComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReporteContratoRoutingModule { }
