import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SolpComponent } from './solp/solp.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ListadoDashboardCompradorComponent } from './dashboard-comprador/listado-dashboard-comprador/listado-dashboard-comprador.component';
import { CotizacionFormularioComponent } from './cotizacion/cotizacion-formulario/cotizacion-formulario.component';
import { ListadoDashboardProveedorComponent } from './dashboard-proveedor/listado-dashboard-proveedor/listado-dashboard-proveedor.component';
import { VerOfertasComponent } from './dashboard-comprador/ver-ofertas/ver-ofertas.component';

const routes: Routes = [
    { path: '', component: DashboardComponent },
    { path: "solp", component: SolpComponent },
    { path: "solp/:id", component: SolpComponent },
    { path: "solp/:id/:tipoSolp", component: SolpComponent },
    { path: "dashboard", component: DashboardComponent },
    { path: "dashboardComprador", component: ListadoDashboardCompradorComponent },
    { path: "dashboardProveedor", component: ListadoDashboardProveedorComponent },
    { path: "cotizacion-formulario", component: CotizacionFormularioComponent },
    { path: "cotizacion-formulario/:id", component: CotizacionFormularioComponent },
    { path: "ver-ofertas/:id", component: VerOfertasComponent }


];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ComprasRoutingModule { }
