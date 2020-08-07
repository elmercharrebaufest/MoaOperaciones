import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { AltaUsuarioComponent } from './usuario/alta/usuario.alta.component';
import { CamaraConsolidacionComponent } from './aduana/camara/consolidacion/camara.consolidacion.component';
import { CamaraMuelleComponent } from './aduana/camara/muelle/camara.muelle.component';
import { CargaPesadasComponent } from './ryd/carga-pesadas/carga-pesadas.component';
import { CartaPorteBaseComponent } from './carta-porte/carta-porte.component';
import { CartaPorteAplicacionComponent } from './carta-porte/aplicacion/carta-porte.aplicacion2.component';
import { CartaPorteDetalleComponent } from './carta-porte/detalle/carta-porte.detalle2.component';
import { CartaPorteDescargaComponent } from './carta-porte/descarga/carta-porte.descarga2.component';
import { CartaPorteFormularioComponent } from './carta-porte/formulario/carta-porte.formulario.component';
import { CambioContraseniaComponent } from './usuario/cambio-contrasenia/usuario.cambio-contrasenia.component';
import { ContratoAmpliacionComponent } from './contrato/ampliacion/contrato.ampliacion.component';
import { ContratoAnulacionComponent } from './contrato/anulacion/contrato.anulacion.component';
import { ContratoFijacionComponent } from './contrato/fijacion/contrato.fijacion.component';
import { ContratoVigenteComponent } from './contrato/vigente/contrato.vigente.component';
import { ContratoDetalleComponent } from './contrato/detalle/contrato.detalle.component';
import { ContratoDetalleFijacionComponent } from './contrato/detalle-fijacion/contrato.detalle-fijacion.component';
import { ContactoMailComponent } from './contacto-mail/contacto-mail.component';
import { CuentaCorrienteBaseComponent } from './cuenta-corriente/cuenta-corriente.component';
import { CuentaCorrienteAgrupadaComponent } from './cuenta-corriente/agrupada/cuenta-corriente.agrupada.component';
import { DatoFiscalBaseComponent } from './dato-fiscal/dato-fiscal.component';
import { DocumentacionComponent } from './dato-fiscal/documentacion/documentacion.component';
import { FleteBaseComponent } from './flete/flete.component';
import { FleteAFacturarComponent } from './flete/a-facturar/flete.a-facturar.component';
import { FleteFacturadoComponent } from './flete/facturado/flete.facturado.component';
import { FletePendienteComponent } from './flete/pendiente/flete.pendiente.component';
import { HomeComponent } from './home/home.component';
import { HomeNGSComponent } from './home/no-granos/home.no-granos.component';
import { InformeComponent } from './ryd/informe/informe.component';
import { InformacionMeteorologicaComponent } from './aduana/informacion-meteorologica/informacion-meteorologica.component';
import { LayoutComponent } from './layout/layout.component';
import { LiquidacionBaseComponent } from './liquidacion/liquidacion.component';
import { LiquidacionAprobadaComponent } from './liquidacion/aprobada/liquidacion.aprobada.component';
import { LiquidacionObservadaComponent } from './liquidacion/observada/liquidacion.observada.component';
import { LiquidacionPagaComponent } from './liquidacion/paga/liquidacion.paga.component';
import { LiquidacionNGAprobadaComponent } from './liquidacion/no-granos/aprobada/liquidacion.no-granos.aprobada.component';
import { LiquidacionNGObservadaComponent } from './liquidacion/no-granos/observada/liquidacion.no-granos.observada.component';
import { LiquidacionNGPagaComponent } from './liquidacion/no-granos/paga/liquidacion.no-granos.paga.component';
import { LiquidacionProformaComponent } from './liquidacion/proforma/liquidacion.proforma.component';
import { ListadoPesadasComponent } from './ryd/listado-pesadas/listado-pesadas.component';
import { LoginGuard } from './common/security/login-guard';
import { LoginComponent } from './login/login.component';
import { NoAutorizadoComponent } from './error/error.no-autorizado.component';
import { PagoComponent } from './pago/pago.component';
import { PagoEmitidoNGSComponent } from './pago/no-granos/emitido/pago.no-granos.emitido.component';
import { PagoEmitidoComponent } from './pago/emitido/pago.emitido.component';
import { PagoDetalleComponent } from './pago/detalle/pago.detalle.component';
import { PesadaDetalleComponent } from './aduana/pesada/detalle/pesada.detalle.component';
import { PesadaHistoricaComponent } from './aduana/pesada/historica/pesada.historica.component';
import { PesadaOnlineComponent } from './aduana/pesada/online/pesada.online.component';
import { RYDBaseComponent } from './ryd/ryd.component';
import { RecuperarContraseniaComponent } from './login/recuperar-contrasenia/login.recuperar-contrasenia.component';
import { RegistroUsuarioComponent } from './login/registro/login.registro.component';
import { RYDMantenimientoBaseComponent } from './ryd-mantenimiento/ryd-mantenimiento.component';
import { RYDMantenimientoBalanzaComponent } from './ryd-mantenimiento/balanzas/balanzas.component';
import { RYDMantenimientoCommoditiesComponent } from './ryd-mantenimiento/commodities/commodities.component';
import { RYDMantenimientoExportadorComponent } from './ryd-mantenimiento/exportadores/exportadores.component';
import { UsuarioListComponent } from './usuario/list/usuario.list.component';
import { UsuarioCambioVendedorComponent } from './usuario/cambio-vendedor/usuario.cambio-vendedor.component';
import { VendedoresListComponent } from './dato-fiscal/vendedor/dato-fiscal.vendedor.component';
import { VendedorStatusComponent } from './vendedor/vendedor_status.component';
import { FacturaComponent } from './factura/factura.component';
import { PesificacionComponent } from './pesificacion/pesificacion.component';
import { EmpresaGranosComponent } from './alta-proveedores/empresa-granos/empresa-granos.component';
import { EmpresaNoGranosComponent } from './alta-proveedores/empresa-no-granos/empresa-no-granos.component';
import { AltasComponent } from './alta-proveedores/altas/altas.component';
import { ProveedorDetalleComponent } from './alta-proveedores/proveedor-detalle/proveedor-detalle.component';


const appRoutes: Routes = [
    { path: 'login', component: LoginComponent, canActivate: [LoginGuard] },
    { path: 'documentacion', component: DocumentacionComponent },
    { path: 'usuario/registro', component: RegistroUsuarioComponent, canActivate: [LoginGuard] },
    { path: 'usuario/recuperar-contrasenia', component: RecuperarContraseniaComponent, canActivate: [LoginGuard] },
    {
        path: '',
        component: LayoutComponent,
        canActivateChild: [LoginGuard],
        children: [
            { path: '', component: HomeComponent },
            { path: 'aduana/pesada/detalle', component: PesadaDetalleComponent },
            { path: 'aduana/pesada-online', component: PesadaOnlineComponent },
            { path: 'aduana/pesada-historica', component: PesadaHistoricaComponent },
            { path: 'aduana/informacion-meteorologica', component: InformacionMeteorologicaComponent },
            { path: 'aduana/camara-consolidacion', component: CamaraConsolidacionComponent },
            { path: 'aduana/camara-muelle', component: CamaraMuelleComponent },
            { path: 'carta-porte', component: CartaPorteBaseComponent },
            { path: 'carta-porte/descarga', component: CartaPorteDescargaComponent },
            { path: 'carta-porte/aplicacion', component: CartaPorteAplicacionComponent },
            { path: 'carta-porte/detalle/:id', component: CartaPorteDetalleComponent },
            { path: 'carta-porte/formulario', component: CartaPorteFormularioComponent },
            { path: 'comprobante-ngs', component: LiquidacionNGAprobadaComponent },
            { path: 'comprobante-ngs/aprobada', component: LiquidacionNGAprobadaComponent },
            { path: 'comprobante-ngs/observada', component: LiquidacionNGObservadaComponent },
            /*{ path: 'comprobante-ngs/paga', component: LiquidacionNGPagaComponent },*/
            { path: 'contrato', component: ContratoVigenteComponent },
            { path: 'contrato/vigente', component: ContratoVigenteComponent },
            { path: 'contrato/fijacion', component: ContratoFijacionComponent },
            { path: 'contrato/ampliacion', component: ContratoAmpliacionComponent },
            { path: 'contrato/anulacion', component: ContratoAnulacionComponent },
            { path: 'contrato/detalle/:id', component: ContratoDetalleComponent },
            { path: 'contrato/detalle-fijacion/:id/:id2', component: ContratoDetalleFijacionComponent },
            { path: 'contacto', component: ContactoMailComponent },
            { path: 'cuenta-corriente', component: CuentaCorrienteBaseComponent },
            { path: 'cuenta-corriente/simple', component: CuentaCorrienteBaseComponent },
            { path: 'cuenta-corriente/simple/:id', component: CuentaCorrienteBaseComponent },
            { path: 'cuenta-corriente/agrupada', component: CuentaCorrienteAgrupadaComponent },
            { path: 'cuenta-corriente/agrupada/:id', component: CuentaCorrienteAgrupadaComponent },
            { path: 'dato-fiscal/situacion-fiscal', component: DatoFiscalBaseComponent },
            { path: 'dato-fiscal/situacion-fiscal/:id', component: DatoFiscalBaseComponent },
            { path: 'dato-fiscal/situacion-fiscal/:id/:id2', component: DatoFiscalBaseComponent },
            { path: 'dato-fiscal/documentacion', component: DocumentacionComponent },
            { path: 'dato-fiscal/vendedor', component: VendedoresListComponent },
            { path: 'flete', component: FleteAFacturarComponent },
            { path: 'flete/a-facturar', component: FleteAFacturarComponent },
            { path: 'flete/facturado', component: FleteFacturadoComponent },
            { path: 'flete/pendiente', component: FletePendienteComponent },
            { path: 'home', component: HomeComponent },
            { path: 'home-ngs', component: HomeNGSComponent },
            { path: 'liquidacion', component: LiquidacionAprobadaComponent },
            { path: 'liquidacion/aprobada', component: LiquidacionAprobadaComponent },
            { path: 'liquidacion/observada', component: LiquidacionObservadaComponent },
            { path: 'liquidacion/paga', component: LiquidacionPagaComponent },
            { path: 'liquidacion/proforma/:id', component: LiquidacionProformaComponent },
            { path: 'no-autorizado', component: NoAutorizadoComponent },
            { path: 'pago', component: PagoComponent },
            { path: 'pago-ngs/emitido', component: PagoEmitidoNGSComponent },
            { path: 'pago/emitido', component: PagoEmitidoComponent },
            { path: 'pago/detalle/:id', component: PagoDetalleComponent },
            { path: 'ryd', component: CargaPesadasComponent },
            { path: 'ryd/carga-pesada', component: CargaPesadasComponent },
            { path: 'ryd/informe', component: InformeComponent },
            { path: 'ryd/listado-pesadas', component: ListadoPesadasComponent },
            { path: 'ryd-mantenimiento/balanzas', component: RYDMantenimientoBalanzaComponent },
            { path: 'ryd-mantenimiento/commodities', component: RYDMantenimientoCommoditiesComponent },
            { path: 'ryd-mantenimiento/exportadores', component: RYDMantenimientoExportadorComponent },
            { path: 'usuario/alta', component: AltaUsuarioComponent },
            { path: 'usuario/list', component: UsuarioListComponent },
            { path: 'usuario/cambio-contrasenia', component: CambioContraseniaComponent },
            { path: 'usuario/cambio-vendedor', component: UsuarioCambioVendedorComponent },
            { path: 'ryd-mantenimiento', component: RYDMantenimientoBaseComponent },
            { path: 'vendedor/status', component: VendedorStatusComponent },
            { path: 'factura', component: FacturaComponent },
            { path: 'pesificacion', component: PesificacionComponent },
            { path: 'alta-empresa-granos', component: EmpresaGranosComponent },
            { path: 'alta-empresa-no-granos', component: EmpresaNoGranosComponent },
            { path: 'altas', component: AltasComponent },
            { path: 'proveedor-detalle', component: ProveedorDetalleComponent }
        ],
    },
    { path: '**', component: LoginComponent },

];

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [
        RouterModule
    ]
})
export class AppRoutingModule { }