
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SolpComponent } from './solp/solp.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ListadoDashboardCompradorComponent } from './dashboard-comprador/listado-dashboard-comprador/listado-dashboard-comprador.component';

import { ListadoDashboardProveedorComponent } from './dashboard-proveedor/listado-dashboard-proveedor/listado-dashboard-proveedor.component';
import { PeticionDeOfertaFormularioComponent } from './peticion-de-oferta-formulario/peticion-de-oferta-formulario.component';
import { CotizacionFormularioComponent } from './dashboard-proveedor/cotizacion-formulario/cotizacion-formulario.component';
import { VerOfertasComponent } from './dashboard-comprador/ver-ofertas/ver-ofertas.component';
import { ReporteOcComponent } from './reporte-oc/reporte-oc.component';

import { ListadoDashboardCertificacionDeServiciosComponent } from './dashboard-entrada-de-servicio/listado-dashboard-certificacion-de-servicios/listado-dashboard-certificacion-de-servicios.component';

const routes: Routes = [
    { path: '', component: DashboardComponent },
    { path: "solp", component: SolpComponent },
    { path: "solp/:id", component: SolpComponent },
    { path: "solp/:id/:tipoSolp", component: SolpComponent },
    { path: "dashboard", component: DashboardComponent },
    { path: "dashboardComprador", component: ListadoDashboardCompradorComponent },
    { path: "dashboardProveedor", component: ListadoDashboardProveedorComponent },
    { path: "dashboardCertificacionDeServicios", component: ListadoDashboardCertificacionDeServiciosComponent },
    { path: "peticion-de-oferta-formulario", component: PeticionDeOfertaFormularioComponent },
    { path: "peticion-de-oferta-formulario/:id", component: PeticionDeOfertaFormularioComponent },
    { path: "dashboard-proveedor/cotizacion", component: CotizacionFormularioComponent },
    { path: "dashboard-proveedor/cotizacion/:id", component: CotizacionFormularioComponent },
    { path: "ver-ofertas/:id", component: VerOfertasComponent },
    { path: "reporte-oc", component: ReporteOcComponent }



];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ComprasRoutingModule { }
