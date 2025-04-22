
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SolpComponent } from './solp/solp.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ListadoDashboardCompradorComponent } from './dashboard-comprador/listado-dashboard-comprador/listado-dashboard-comprador.component';

import { ListadoDashboardProveedorComponent } from './dashboard-proveedor/listado-dashboard-proveedor/listado-dashboard-proveedor.component';
import { PeticionDeOfertaFormularioComponent } from './peticion-de-oferta-formulario/peticion-de-oferta-formulario.component';
import { CotizacionFormularioComponent } from './dashboard-proveedor/cotizacion-formulario/cotizacion-formulario.component';
import { VerOfertasComponent } from './dashboard-comprador/ver-ofertas/ver-ofertas.component';
import { ListadoEstadoCertificacionesComponent } from './dashboard-entrada-de-servicio/listado-estado-certificaciones/listado-estado-certificaciones.component';
import { ReporteOcComponent } from './reporte-oc/reporte-oc.component';
import { CrearPoMultipleComponent } from './crear-po-multiple/crear-po-multiple.component';
import { AgruparPoThComponent } from './agrupar-po-th/agrupar-po-th.component';

import { ListadoDashboardCertificacionDeServiciosComponent } from './dashboard-entrada-de-servicio/listado-dashboard-certificacion-de-servicios/listado-dashboard-certificacion-de-servicios.component';
import { ListadoDashboardCertificacionDeServiciosProveedoresComponent } from './dashboard-entrada-de-servicio/listado-dashboard-certificacion-de-servicios-proveedor/listado-dashboard-certificacion-de-servicios-proveedor.component';
import { ListadoEstadoCertificacionesProveedorComponent } from './dashboard-entrada-de-servicio/listado-estado-certificaciones-proveedor/listado-estado-certificaciones-proveedor.component';
import { DashboardPliegoMultipleComponent } from './dashboard-pliego-multiple/dashboard-pliego-multiple.component';
import { ReporteFacturasCertificacionesComponent } from './reporte-facturas-certificaciones/reporte-facturas-certificaciones.component';

const routes: Routes = [
    { path: '', component: DashboardComponent },
    { path: "solp", component: SolpComponent },
    { path: "solp/:id", component: SolpComponent },
    { path: "solp/:id/:tipoSolp", component: SolpComponent },
    { path: "solp/:id/:tipoSolp/:action", component: SolpComponent },
    { path: "dashboard", component: DashboardComponent },
    { path: "dashboardComprador", component: ListadoDashboardCompradorComponent },
    { path: "dashboardProveedor", component: ListadoDashboardProveedorComponent },
    { path: "listadoEstadoCertificaciones", component: ListadoEstadoCertificacionesComponent },
    { path: "listadoEstadoCertificacionesProveedor", component: ListadoEstadoCertificacionesProveedorComponent },
    { path: "dashboardCertificacionDeServicios", component: ListadoDashboardCertificacionDeServiciosComponent },
    { path: "dashboardCertificacionDeServiciosProveedores", component: ListadoDashboardCertificacionDeServiciosProveedoresComponent },
    { path: "dashboardCertificacionDeServiciosProveedores/:ordenCompraId", component: ListadoDashboardCertificacionDeServiciosProveedoresComponent },
    { path: "peticion-de-oferta-formulario", component: PeticionDeOfertaFormularioComponent },
    { path: "peticion-de-oferta-formulario/:id", component: PeticionDeOfertaFormularioComponent },
    { path: "dashboard-proveedor/cotizacion", component: CotizacionFormularioComponent },
    { path: "dashboard-proveedor/cotizacion/:id", component: CotizacionFormularioComponent },
    { path: "ver-ofertas/:id", component: VerOfertasComponent },
    { path: "reporte-oc", component: ReporteOcComponent },
    { path: "crear-po-multiple", component: CrearPoMultipleComponent },
    { path: "agrupar-po-th", component: AgruparPoThComponent },
    { path: "dashboardPliegoMultiple", component: DashboardPliegoMultipleComponent },
    { path: "reporteFacturasCertificaciones", component: ReporteFacturasCertificacionesComponent },

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ComprasRoutingModule { }
