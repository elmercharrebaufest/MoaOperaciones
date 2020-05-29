"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var usuario_alta_component_1 = require("./usuario/alta/usuario.alta.component");
var camara_consolidacion_component_1 = require("./aduana/camara/consolidacion/camara.consolidacion.component");
var camara_muelle_component_1 = require("./aduana/camara/muelle/camara.muelle.component");
var carga_pesadas_component_1 = require("./ryd/carga-pesadas/carga-pesadas.component");
var carta_porte_component_1 = require("./carta-porte/carta-porte.component");
var carta_porte_aplicacion_component_1 = require("./carta-porte/aplicacion/carta-porte.aplicacion.component");
var carta_porte_detalle_component_1 = require("./carta-porte/detalle/carta-porte.detalle.component");
var carta_porte_descarga_component_1 = require("./carta-porte/descarga/carta-porte.descarga.component");
var carta_porte_formulario_component_1 = require("./carta-porte/formulario/carta-porte.formulario.component");
var usuario_cambio_contrasenia_component_1 = require("./usuario/cambio-contrasenia/usuario.cambio-contrasenia.component");
var contrato_ampliacion_component_1 = require("./contrato/ampliacion/contrato.ampliacion.component");
var contrato_anulacion_component_1 = require("./contrato/anulacion/contrato.anulacion.component");
var contrato_fijacion_component_1 = require("./contrato/fijacion/contrato.fijacion.component");
var contrato_vigente_component_1 = require("./contrato/vigente/contrato.vigente.component");
var contrato_detalle_component_1 = require("./contrato/detalle/contrato.detalle.component");
var contrato_detalle_fijacion_component_1 = require("./contrato/detalle-fijacion/contrato.detalle-fijacion.component");
var contacto_mail_component_1 = require("./contacto-mail/contacto-mail.component");
var cuenta_corriente_component_1 = require("./cuenta-corriente/cuenta-corriente.component");
var cuenta_corriente_agrupada_component_1 = require("./cuenta-corriente/agrupada/cuenta-corriente.agrupada.component");
var dato_fiscal_component_1 = require("./dato-fiscal/dato-fiscal.component");
var documentacion_component_1 = require("./dato-fiscal/documentacion/documentacion.component");
var flete_a_facturar_component_1 = require("./flete/a-facturar/flete.a-facturar.component");
var flete_facturado_component_1 = require("./flete/facturado/flete.facturado.component");
var flete_pendiente_component_1 = require("./flete/pendiente/flete.pendiente.component");
var home_component_1 = require("./home/home.component");
var home_no_granos_component_1 = require("./home/no-granos/home.no-granos.component");
var informe_component_1 = require("./ryd/informe/informe.component");
var informacion_meteorologica_component_1 = require("./aduana/informacion-meteorologica/informacion-meteorologica.component");
var layout_component_1 = require("./layout/layout.component");
var liquidacion_aprobada_component_1 = require("./liquidacion/aprobada/liquidacion.aprobada.component");
var liquidacion_observada_component_1 = require("./liquidacion/observada/liquidacion.observada.component");
var liquidacion_paga_component_1 = require("./liquidacion/paga/liquidacion.paga.component");
var liquidacion_no_granos_aprobada_component_1 = require("./liquidacion/no-granos/aprobada/liquidacion.no-granos.aprobada.component");
var liquidacion_no_granos_observada_component_1 = require("./liquidacion/no-granos/observada/liquidacion.no-granos.observada.component");
var liquidacion_proforma_component_1 = require("./liquidacion/proforma/liquidacion.proforma.component");
var listado_pesadas_component_1 = require("./ryd/listado-pesadas/listado-pesadas.component");
var login_guard_1 = require("./common/security/login-guard");
var login_component_1 = require("./login/login.component");
var error_no_autorizado_component_1 = require("./error/error.no-autorizado.component");
var pago_component_1 = require("./pago/pago.component");
var pago_no_granos_emitido_component_1 = require("./pago/no-granos/emitido/pago.no-granos.emitido.component");
var pago_emitido_component_1 = require("./pago/emitido/pago.emitido.component");
var pago_detalle_component_1 = require("./pago/detalle/pago.detalle.component");
var pesada_detalle_component_1 = require("./aduana/pesada/detalle/pesada.detalle.component");
var pesada_historica_component_1 = require("./aduana/pesada/historica/pesada.historica.component");
var pesada_online_component_1 = require("./aduana/pesada/online/pesada.online.component");
var login_recuperar_contrasenia_component_1 = require("./login/recuperar-contrasenia/login.recuperar-contrasenia.component");
var login_registro_component_1 = require("./login/registro/login.registro.component");
var ryd_mantenimiento_component_1 = require("./ryd-mantenimiento/ryd-mantenimiento.component");
var balanzas_component_1 = require("./ryd-mantenimiento/balanzas/balanzas.component");
var commodities_component_1 = require("./ryd-mantenimiento/commodities/commodities.component");
var exportadores_component_1 = require("./ryd-mantenimiento/exportadores/exportadores.component");
var usuario_list_component_1 = require("./usuario/list/usuario.list.component");
var usuario_cambio_vendedor_component_1 = require("./usuario/cambio-vendedor/usuario.cambio-vendedor.component");
var dato_fiscal_vendedor_component_1 = require("./dato-fiscal/vendedor/dato-fiscal.vendedor.component");
var vendedor_status_component_1 = require("./vendedor/vendedor_status.component");
var factura_component_1 = require("./factura/factura.component");
var pesificacion_component_1 = require("./pesificacion/pesificacion.component");
var appRoutes = [
    { path: 'login', component: login_component_1.LoginComponent, canActivate: [login_guard_1.LoginGuard] },
    { path: 'documentacion', component: documentacion_component_1.DocumentacionComponent },
    { path: 'usuario/registro', component: login_registro_component_1.RegistroUsuarioComponent, canActivate: [login_guard_1.LoginGuard] },
    { path: 'usuario/recuperar-contrasenia', component: login_recuperar_contrasenia_component_1.RecuperarContraseniaComponent, canActivate: [login_guard_1.LoginGuard] },
    {
        path: '',
        component: layout_component_1.LayoutComponent,
        canActivateChild: [login_guard_1.LoginGuard],
        children: [
            { path: '', component: home_component_1.HomeComponent },
            { path: 'aduana/pesada/detalle', component: pesada_detalle_component_1.PesadaDetalleComponent },
            { path: 'aduana/pesada-online', component: pesada_online_component_1.PesadaOnlineComponent },
            { path: 'aduana/pesada-historica', component: pesada_historica_component_1.PesadaHistoricaComponent },
            { path: 'aduana/informacion-meteorologica', component: informacion_meteorologica_component_1.InformacionMeteorologicaComponent },
            { path: 'aduana/camara-consolidacion', component: camara_consolidacion_component_1.CamaraConsolidacionComponent },
            { path: 'aduana/camara-muelle', component: camara_muelle_component_1.CamaraMuelleComponent },
            { path: 'carta-porte', component: carta_porte_component_1.CartaPorteBaseComponent },
            { path: 'carta-porte/descarga', component: carta_porte_descarga_component_1.CartaPorteDescargaComponent },
            { path: 'carta-porte/aplicacion', component: carta_porte_aplicacion_component_1.CartaPorteAplicacionComponent },
            { path: 'carta-porte/detalle/:id', component: carta_porte_detalle_component_1.CartaPorteDetalleComponent },
            { path: 'carta-porte/formulario', component: carta_porte_formulario_component_1.CartaPorteFormularioComponent },
            { path: 'comprobante-ngs', component: liquidacion_no_granos_aprobada_component_1.LiquidacionNGAprobadaComponent },
            { path: 'comprobante-ngs/aprobada', component: liquidacion_no_granos_aprobada_component_1.LiquidacionNGAprobadaComponent },
            { path: 'comprobante-ngs/observada', component: liquidacion_no_granos_observada_component_1.LiquidacionNGObservadaComponent },
            /*{ path: 'comprobante-ngs/paga', component: LiquidacionNGPagaComponent },*/
            { path: 'contrato', component: contrato_vigente_component_1.ContratoVigenteComponent },
            { path: 'contrato/vigente', component: contrato_vigente_component_1.ContratoVigenteComponent },
            { path: 'contrato/fijacion', component: contrato_fijacion_component_1.ContratoFijacionComponent },
            { path: 'contrato/ampliacion', component: contrato_ampliacion_component_1.ContratoAmpliacionComponent },
            { path: 'contrato/anulacion', component: contrato_anulacion_component_1.ContratoAnulacionComponent },
            { path: 'contrato/detalle/:id', component: contrato_detalle_component_1.ContratoDetalleComponent },
            { path: 'contrato/detalle-fijacion/:id/:id2', component: contrato_detalle_fijacion_component_1.ContratoDetalleFijacionComponent },
            { path: 'contacto', component: contacto_mail_component_1.ContactoMailComponent },
            { path: 'cuenta-corriente', component: cuenta_corriente_component_1.CuentaCorrienteBaseComponent },
            { path: 'cuenta-corriente/simple', component: cuenta_corriente_component_1.CuentaCorrienteBaseComponent },
            { path: 'cuenta-corriente/simple/:id', component: cuenta_corriente_component_1.CuentaCorrienteBaseComponent },
            { path: 'cuenta-corriente/agrupada', component: cuenta_corriente_agrupada_component_1.CuentaCorrienteAgrupadaComponent },
            { path: 'cuenta-corriente/agrupada/:id', component: cuenta_corriente_agrupada_component_1.CuentaCorrienteAgrupadaComponent },
            { path: 'dato-fiscal/situacion-fiscal', component: dato_fiscal_component_1.DatoFiscalBaseComponent },
            { path: 'dato-fiscal/situacion-fiscal/:id', component: dato_fiscal_component_1.DatoFiscalBaseComponent },
            { path: 'dato-fiscal/situacion-fiscal/:id/:id2', component: dato_fiscal_component_1.DatoFiscalBaseComponent },
            { path: 'dato-fiscal/documentacion', component: documentacion_component_1.DocumentacionComponent },
            { path: 'dato-fiscal/vendedor', component: dato_fiscal_vendedor_component_1.VendedoresListComponent },
            { path: 'flete', component: flete_a_facturar_component_1.FleteAFacturarComponent },
            { path: 'flete/a-facturar', component: flete_a_facturar_component_1.FleteAFacturarComponent },
            { path: 'flete/facturado', component: flete_facturado_component_1.FleteFacturadoComponent },
            { path: 'flete/pendiente', component: flete_pendiente_component_1.FletePendienteComponent },
            { path: 'home', component: home_component_1.HomeComponent },
            { path: 'home-ngs', component: home_no_granos_component_1.HomeNGSComponent },
            { path: 'liquidacion', component: liquidacion_aprobada_component_1.LiquidacionAprobadaComponent },
            { path: 'liquidacion/aprobada', component: liquidacion_aprobada_component_1.LiquidacionAprobadaComponent },
            { path: 'liquidacion/observada', component: liquidacion_observada_component_1.LiquidacionObservadaComponent },
            { path: 'liquidacion/paga', component: liquidacion_paga_component_1.LiquidacionPagaComponent },
            { path: 'liquidacion/proforma/:id', component: liquidacion_proforma_component_1.LiquidacionProformaComponent },
            { path: 'no-autorizado', component: error_no_autorizado_component_1.NoAutorizadoComponent },
            { path: 'pago', component: pago_component_1.PagoComponent },
            { path: 'pago-ngs/emitido', component: pago_no_granos_emitido_component_1.PagoEmitidoNGSComponent },
            { path: 'pago/emitido', component: pago_emitido_component_1.PagoEmitidoComponent },
            { path: 'pago/detalle/:id', component: pago_detalle_component_1.PagoDetalleComponent },
            { path: 'ryd', component: carga_pesadas_component_1.CargaPesadasComponent },
            { path: 'ryd/carga-pesada', component: carga_pesadas_component_1.CargaPesadasComponent },
            { path: 'ryd/informe', component: informe_component_1.InformeComponent },
            { path: 'ryd/listado-pesadas', component: listado_pesadas_component_1.ListadoPesadasComponent },
            { path: 'ryd-mantenimiento/balanzas', component: balanzas_component_1.RYDMantenimientoBalanzaComponent },
            { path: 'ryd-mantenimiento/commodities', component: commodities_component_1.RYDMantenimientoCommoditiesComponent },
            { path: 'ryd-mantenimiento/exportadores', component: exportadores_component_1.RYDMantenimientoExportadorComponent },
            { path: 'usuario/alta', component: usuario_alta_component_1.AltaUsuarioComponent },
            { path: 'usuario/list', component: usuario_list_component_1.UsuarioListComponent },
            { path: 'usuario/cambio-contrasenia', component: usuario_cambio_contrasenia_component_1.CambioContraseniaComponent },
            { path: 'usuario/cambio-vendedor', component: usuario_cambio_vendedor_component_1.UsuarioCambioVendedorComponent },
            { path: 'ryd-mantenimiento', component: ryd_mantenimiento_component_1.RYDMantenimientoBaseComponent },
            { path: 'vendedor/status', component: vendedor_status_component_1.VendedorStatusComponent },
            { path: 'factura', component: factura_component_1.FacturaComponent },
            { path: 'pesificacion', component: pesificacion_component_1.PesificacionComponent }
        ],
    },
    { path: '**', component: login_component_1.LoginComponent },
];
var AppRoutingModule = /** @class */ (function () {
    function AppRoutingModule() {
    }
    AppRoutingModule = __decorate([
        core_1.NgModule({
            imports: [
                router_1.RouterModule.forRoot(appRoutes)
            ],
            exports: [
                router_1.RouterModule
            ]
        })
    ], AppRoutingModule);
    return AppRoutingModule;
}());
exports.AppRoutingModule = AppRoutingModule;
//# sourceMappingURL=app-routing.module.js.map