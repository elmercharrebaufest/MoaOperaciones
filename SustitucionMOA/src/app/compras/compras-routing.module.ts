import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SolpComponent } from './solp/solp.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ListadoDashboardCompradorComponent } from './dashboard-comprador/listado-dashboard-comprador/listado-dashboard-comprador.component';
import { CotizacionFormularioComponent } from './cotizacion/cotizacion-formulario/cotizacion-formulario.component';

const routes: Routes = [
    { path: '', component: DashboardComponent },
    { path: "solp", component: SolpComponent },
    { path: "solp/:id", component: SolpComponent },
    { path: "solp/:id/:tipoSolp", component: SolpComponent },
    { path: "dashboard", component: DashboardComponent },
    { path: "dashboardComprador", component: ListadoDashboardCompradorComponent },
    { path: "cotizacion-formulario", component: CotizacionFormularioComponent },
    { path: "cotizacion-formulario/:id", component: CotizacionFormularioComponent }


];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ComprasRoutingModule { }
