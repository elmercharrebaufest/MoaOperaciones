import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AltasComponent } from "./alta-proveedores/altas/altas.component";
import { EmpresaGranosComponent } from "./alta-proveedores/empresa-granos/empresa-granos.component";
import { EmpresaNoGranosComponent } from "./alta-proveedores/empresa-no-granos/empresa-no-granos.component";
import { EstadoSolicitudComponent } from "./alta-proveedores/estado-solicitud/estado-solicitud.component";
import { LoginGuard } from "./common/security/login-guard";
import { ContactoMailComponent } from "./contacto-mail/contacto-mail.component";
import { NoAutorizadoComponent } from "./error/error.no-autorizado.component";
import { HomeComponent } from "./home/home.component";
import { HomeNGSComponent } from "./home/no-granos/home.no-granos.component";
import { LayoutComponent } from "./layout/layout.component";
import { AltaNotificacionesComponent } from "./notificaciones/alta-notificaciones/alta-notificaciones.component";
import { ListadoNotificacionesComponent } from "./notificaciones/listado-notificaciones/listado-notificaciones.component";
import { PesificacionComponent } from "./pesificacion/pesificacion.component";
import { UsuarioCambioVendedorComponent } from "./usuario/cambio-vendedor/usuario.cambio-vendedor.component";
import { UsuarioListComponent } from "./usuario/list/usuario.list.component";
import { VendedorStatusComponent } from "./vendedor/vendedor_status.component";
import { FaqComponent } from "./faq/faq.component"
import { UsuarioAltaEmpresaNoGranosComponent } from "./usuario/alta-empresa-no-granos/usuario.alta-empresa-no-granos.component";
import { TicketPesadaComponent } from "./ticket-pesada/ticket-pesada.component";
import { ApikeyComponent } from "./apikey/apikey.component";
import { PesificacionesGuardadasComponent } from "./pesificacion/pesificaciones-guardadas/pesificaciones-guardadas.component";
import { LegajoExternoComponent } from './compras/legajo-externo/legajo-externo.component';
import { ConfirmDeactivated } from "./common/security/canDeactive-guard";

const appRoutes: Routes = [
    // { path: "documentacion", component: DocumentacionComponent },
    // {
    //   path: "usuario/registro",
    //   component: RegistroUsuarioComponent,
    //   canActivate: [LoginGuard],
    // },
    // {
    //   path: "usuario/recuperar-contrasenia",
    //   component: RecuperarContraseniaComponent,
    //   canActivate: [LoginGuard],
    // },
    { path: "ticket-pesada", component: TicketPesadaComponent },
    { path: "verLegajoOrdenDeCompra/:id/:token", component: LegajoExternoComponent },

    {
        path: "",
        component: LayoutComponent,
        canActivateChild: [LoginGuard],
        children: [
            { path: "", component: HomeComponent },
            // { path: "aduana/pesada/detalle", component: PesadaDetalleComponent },
            // { path: "aduana/pesada-online", component: PesadaOnlineComponent },
            // { path: "aduana/pesada-historica", component: PesadaHistoricaComponent },
            // {
            //   path: "aduana/informacion-meteorologica",
            //   component: InformacionMeteorologicaComponent,
            // },
            // {
            //   path: "aduana/camara-consolidacion",
            //   component: CamaraConsolidacionComponent,
            // },
            // { path: "aduana/camara-muelle", component: CamaraMuelleComponent },
            {
                path: "carta-porte",
                loadChildren: "./carta-porte/carta-porte.module#CartaPorteModule",
            },

            {
                path: "comprobante-ngs",
                loadChildren:
                    "./liquidacion/no-granos/liquidacion-no-granos.module#LiquidacionNoGranosModule",
            },
            {
                path: "contrato",
                loadChildren: "./contrato/contrato.module#ContratoModule",
            },
            {
                path: "reporte",
                loadChildren: "./reporte/reporte.module#ReporteModule",
            },
            { path: "contacto", component: ContactoMailComponent },
            {
                path: "cuenta-corriente",
                loadChildren:
                    "./cuenta-corriente/cuenta-corriente.module#CuentaCorrienteModule",
            },
            {
                path: "dato-fiscal",
                loadChildren: "./dato-fiscal/dato-fiscal.module#DatoFiscalModule",
            },
            { path: "flete", loadChildren: "./flete/flete.module#FleteModule" },
            { path: "home", component: HomeComponent },
            { path: "home-ngs", component: HomeNGSComponent },
            {
                path: "liquidacion",
                loadChildren: "./liquidacion/liquidacion.module#LiquidacionModule",
            },
            { path: "no-autorizado", component: NoAutorizadoComponent },
            { path: "pago", loadChildren: "./pago/pago.module#PagoModule" },
            {
                path: "pago-ngs",
                loadChildren:
                    "./pago/no-granos/pago-no-granos.module#PagoNoGranosModule",
            },
            // { path: "usuario/alta", component: AltaUsuarioComponent },
            { path: "usuario/list", component: UsuarioListComponent },
            { path: "usuario/alta-empresa-no-granos", component: UsuarioAltaEmpresaNoGranosComponent },
            { path: "usuario/alta-empresa-no-granos/:id/:cuit/:mail", component: UsuarioAltaEmpresaNoGranosComponent },
            // {
            //   path: "usuario/cambio-contrasenia",
            //   component: CambioContraseniaComponent,
            // },
            {
                path: "usuario/cambio-vendedor",
                component: UsuarioCambioVendedorComponent,
            },
            { path: "vendedor/status", component: VendedorStatusComponent },
            {
                path: "factura",
                loadChildren: "./factura/factura.module#FacturaModule",
            },
            { path: "pesificacion", component: PesificacionComponent },
            { path: "pesificacion/listado", component: PesificacionesGuardadasComponent },
            { path: "pesificacion/listado/:id", component: PesificacionesGuardadasComponent },
            { path: "alta-empresa-granos", component: EmpresaGranosComponent },
            {
                path: "alta-empresa-granos/:id",
                component: EmpresaGranosComponent,
            },
            { path: "estado-solicitud", component: EstadoSolicitudComponent },
            { path: "alta-empresa-no-granos", component: EmpresaNoGranosComponent },
            { path: "alta-empresa-no-granos/:id", component: EmpresaNoGranosComponent, canDeactivate: [ConfirmDeactivated] },
            { path: "altas", component: AltasComponent },
            {
                path: "crear-contrato",
                loadChildren: "./crear-contrato/crear-contrato.module#CrearContratoModule",
            },

            {
                path: "sustentable",
                loadChildren: "./venta-sustentable/venta-sustentable.module#VentaSustentableModule",
            },
            {
                path: "ordenes-de-carga",
                loadChildren: "./ordenes-de-carga/ordenes-de-carga.module#OrdenesDeCargaModule",
            },
            {
                path: "ordenes-de-carga-fason",
                loadChildren: "./ordenes-de-carga-fason/ordenes-de-carga-fason.module#OrdenesDeCargaFasonModule",
            },
            {
                path: "ordenes-residuos",
                loadChildren: "./ordenes-residuos/ordenes-residuos.module#OrdenesResiduosModule"
            },
            {
                path: "reporte-contrato",
                loadChildren: "./reporte-contrato/reporte-contrato.module#ReporteContratoModule",
            },

            { path: "notificaciones", component: ListadoNotificacionesComponent },
            { path: "notificaciones/alta", component: AltaNotificacionesComponent },
            { path: "notificaciones/alta/:id", component: AltaNotificacionesComponent },
            { path: "gestionCM05", loadChildren: "./gestionCM05/gestionCM05.module#GestionCM05Module" },
            {
                path: "faq",
                component: FaqComponent
            },

            {
                path: "consulta",
                loadChildren: "./consulta/consulta.module#ConsultaModule",
            },
            { path: "logPesificacion", loadChildren: "./log-pesificacion/log-pesificacion.module#LogPesificacionModule" },
            { path: "compras", loadChildren: "./compras/compras.module#ComprasModule" },
            { path: "apikey", component: ApikeyComponent },
            {
                path: "echeq",
                loadChildren: "./echeq/echeq.module#EcheqModule",
            },
            {
                path: "aplicaciones-ccpp",
                loadChildren: "./aplicacion-ccpp/aplicacion-ccpp.module#AplicacionCcppModule"
            },
            {
                path: "archivos-boleto",
                loadChildren: "./archivo-boleto/archivo-boleto.module#ArchivoBoletoModule"
            }
        ],
    },
    { path: "**", component: HomeComponent },

];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule],
})
export class AppRoutingModule { }
