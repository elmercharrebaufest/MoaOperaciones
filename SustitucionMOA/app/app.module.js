"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var platform_browser_1 = require("@angular/platform-browser");
var http_1 = require("@angular/http");
var forms_1 = require("@angular/forms");
var common_1 = require("@angular/common");
var app_component_1 = require("./app.component");
var aduana_component_1 = require("./aduana/aduana.component");
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
var contrato_component_1 = require("./contrato/contrato.component");
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
var flete_component_1 = require("./flete/flete.component");
var flete_a_facturar_component_1 = require("./flete/a-facturar/flete.a-facturar.component");
var flete_facturado_component_1 = require("./flete/facturado/flete.facturado.component");
var flete_pendiente_component_1 = require("./flete/pendiente/flete.pendiente.component");
var home_component_1 = require("./home/home.component");
var home_no_granos_component_1 = require("./home/no-granos/home.no-granos.component");
var informe_component_1 = require("./ryd/informe/informe.component");
var informacion_meteorologica_component_1 = require("./aduana/informacion-meteorologica/informacion-meteorologica.component");
var layout_component_1 = require("./layout/layout.component");
var liquidacion_component_1 = require("./liquidacion/liquidacion.component");
var liquidacion_aprobada_component_1 = require("./liquidacion/aprobada/liquidacion.aprobada.component");
var liquidacion_observada_component_1 = require("./liquidacion/observada/liquidacion.observada.component");
var liquidacion_paga_component_1 = require("./liquidacion/paga/liquidacion.paga.component");
var liquidacion_no_granos_aprobada_component_1 = require("./liquidacion/no-granos/aprobada/liquidacion.no-granos.aprobada.component");
var liquidacion_no_granos_observada_component_1 = require("./liquidacion/no-granos/observada/liquidacion.no-granos.observada.component");
var liquidacion_no_granos_paga_component_1 = require("./liquidacion/no-granos/paga/liquidacion.no-granos.paga.component");
var liquidacion_proforma_component_1 = require("./liquidacion/proforma/liquidacion.proforma.component");
var listado_pesadas_component_1 = require("./ryd/listado-pesadas/listado-pesadas.component");
var login_component_1 = require("./login/login.component");
var error_no_autorizado_component_1 = require("./error/error.no-autorizado.component");
var pago_component_1 = require("./pago/pago.component");
var pago_emitido_component_1 = require("./pago/emitido/pago.emitido.component");
var pago_no_granos_emitido_component_1 = require("./pago/no-granos/emitido/pago.no-granos.emitido.component");
var pago_detalle_component_1 = require("./pago/detalle/pago.detalle.component");
var pesada_detalle_component_1 = require("./aduana/pesada/detalle/pesada.detalle.component");
var pesada_historica_component_1 = require("./aduana/pesada/historica/pesada.historica.component");
var pesada_online_component_1 = require("./aduana/pesada/online/pesada.online.component");
var ryd_component_1 = require("./ryd/ryd.component");
var login_recuperar_contrasenia_component_1 = require("./login/recuperar-contrasenia/login.recuperar-contrasenia.component");
var login_registro_component_1 = require("./login/registro/login.registro.component");
var balanzas_component_1 = require("./ryd-mantenimiento/balanzas/balanzas.component");
var ryd_mantenimiento_component_1 = require("./ryd-mantenimiento/ryd-mantenimiento.component");
var commodities_component_1 = require("./ryd-mantenimiento/commodities/commodities.component");
var exportadores_component_1 = require("./ryd-mantenimiento/exportadores/exportadores.component");
var usuario_list_component_1 = require("./usuario/list/usuario.list.component");
var usuario_cambio_vendedor_component_1 = require("./usuario/cambio-vendedor/usuario.cambio-vendedor.component");
var dato_fiscal_vendedor_component_1 = require("./dato-fiscal/vendedor/dato-fiscal.vendedor.component");
var vendedor_status_component_1 = require("./vendedor/vendedor_status.component");
var factura_component_1 = require("./factura/factura.component");
var pesificacion_component_1 = require("./pesificacion/pesificacion.component");
var filtro_fecha_component_1 = require("./common/view-child/filtro-fecha/filtro-fecha.component");
var dropdown_component_1 = require("./common/view-child/dropdown/dropdown.component");
var mensaje_component_1 = require("./common/view-child/mensaje/mensaje.component");
var mensaje_modal_component_1 = require("./common/view-child/mensaje-modal/mensaje-modal.component");
var spinner_component_1 = require("./common/view-child/spinner/spinner.component");
var spinner_small_component_1 = require("./common/view-child/spinner-small/spinner-small.component");
var customFilter_1 = require("./common/pipes/customFilter");
var customFilterOr_1 = require("./common/pipes/customFilterOr");
var customFilterContain_1 = require("./common/pipes/customFilterContain");
var orderedColumn_1 = require("./common/pipes/orderedColumn");
var shortenString_1 = require("./common/pipes/shortenString");
var SessionDataService_1 = require("./common/services/SessionDataService");
var NavService_1 = require("./common/services/NavService");
var DataService_1 = require("./common/services/DataService");
var FloatMsgService_1 = require("./common/services/FloatMsgService");
var aduana_service_1 = require("./aduana/aduana.service");
var ryd_service_1 = require("./ryd/ryd.service");
var layout_service_1 = require("./layout/layout.service");
var liquidacion_service_1 = require("./liquidacion/liquidacion.service");
var pago_service_1 = require("./pago/pago.service");
var cuenta_corriente_service_1 = require("./cuenta-corriente/cuenta-corriente.service");
var usuario_service_1 = require("./usuario/usuario.service");
var ryd_mantenimiento_service_1 = require("./ryd-mantenimiento/ryd-mantenimiento.service");
var flete_service_1 = require("./flete/flete.service");
var vendedor_status_service_1 = require("./vendedor/vendedor_status.service");
var factura_service_1 = require("./factura/factura.service");
var ModalService_1 = require("./common/services/ModalService");
var SecurityService_1 = require("./common/services/SecurityService");
var ServiceLocator_1 = require("./common/services/ServiceLocator");
var login_guard_1 = require("./common/security/login-guard");
var ngx_pagination_1 = require("ngx-pagination");
var ng2_select_1 = require("ng2-select");
var ngx_modal_1 = require("ngx-modal");
var ng2_auto_complete_1 = require("ng2-auto-complete");
var angular2_recaptcha_1 = require("angular2-recaptcha");
var app_routing_module_1 = require("./app-routing.module");
;
var carta_porte_modal_component_1 = require("./carta-porte/carta-porte-modal/carta-porte-modal.component");
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            imports: [
                platform_browser_1.BrowserModule,
                http_1.HttpModule,
                app_routing_module_1.AppRoutingModule,
                forms_1.FormsModule,
                ngx_pagination_1.NgxPaginationModule,
                ng2_select_1.SelectModule,
                ngx_modal_1.ModalModule,
                ng2_auto_complete_1.Ng2AutoCompleteModule,
                angular2_recaptcha_1.ReCaptchaModule
            ],
            declarations: [
                app_component_1.AppComponent,
                aduana_component_1.AduanaBaseComponent,
                usuario_alta_component_1.AltaUsuarioComponent,
                camara_consolidacion_component_1.CamaraConsolidacionComponent,
                camara_muelle_component_1.CamaraMuelleComponent,
                carga_pesadas_component_1.CargaPesadasComponent,
                carta_porte_component_1.CartaPorteBaseComponent,
                carta_porte_aplicacion_component_1.CartaPorteAplicacionComponent,
                carta_porte_detalle_component_1.CartaPorteDetalleComponent,
                carta_porte_descarga_component_1.CartaPorteDescargaComponent,
                carta_porte_formulario_component_1.CartaPorteFormularioComponent,
                usuario_cambio_contrasenia_component_1.CambioContraseniaComponent,
                contrato_component_1.ContratoBaseComponent,
                contrato_ampliacion_component_1.ContratoAmpliacionComponent,
                contrato_anulacion_component_1.ContratoAnulacionComponent,
                contrato_fijacion_component_1.ContratoFijacionComponent,
                contrato_vigente_component_1.ContratoVigenteComponent,
                contrato_detalle_component_1.ContratoDetalleComponent,
                contrato_detalle_fijacion_component_1.ContratoDetalleFijacionComponent,
                contacto_mail_component_1.ContactoMailComponent,
                cuenta_corriente_component_1.CuentaCorrienteBaseComponent,
                cuenta_corriente_agrupada_component_1.CuentaCorrienteAgrupadaComponent,
                dato_fiscal_component_1.DatoFiscalBaseComponent,
                documentacion_component_1.DocumentacionComponent,
                factura_component_1.FacturaComponent,
                flete_component_1.FleteBaseComponent,
                flete_a_facturar_component_1.FleteAFacturarComponent,
                flete_facturado_component_1.FleteFacturadoComponent,
                flete_pendiente_component_1.FletePendienteComponent,
                home_component_1.HomeComponent,
                home_no_granos_component_1.HomeNGSComponent,
                informe_component_1.InformeComponent,
                informacion_meteorologica_component_1.InformacionMeteorologicaComponent,
                layout_component_1.LayoutComponent,
                liquidacion_component_1.LiquidacionBaseComponent,
                liquidacion_aprobada_component_1.LiquidacionAprobadaComponent,
                liquidacion_observada_component_1.LiquidacionObservadaComponent,
                liquidacion_paga_component_1.LiquidacionPagaComponent,
                liquidacion_no_granos_aprobada_component_1.LiquidacionNGAprobadaComponent,
                liquidacion_no_granos_observada_component_1.LiquidacionNGObservadaComponent,
                liquidacion_no_granos_paga_component_1.LiquidacionNGPagaComponent,
                listado_pesadas_component_1.ListadoPesadasComponent,
                liquidacion_proforma_component_1.LiquidacionProformaComponent,
                login_component_1.LoginComponent,
                error_no_autorizado_component_1.NoAutorizadoComponent,
                pago_component_1.PagoComponent,
                pago_emitido_component_1.PagoEmitidoComponent,
                pago_no_granos_emitido_component_1.PagoEmitidoNGSComponent,
                pago_detalle_component_1.PagoDetalleComponent,
                pesada_detalle_component_1.PesadaDetalleComponent,
                pesada_historica_component_1.PesadaHistoricaComponent,
                pesada_online_component_1.PesadaOnlineComponent,
                ryd_component_1.RYDBaseComponent,
                ryd_mantenimiento_component_1.RYDMantenimientoBaseComponent,
                balanzas_component_1.RYDMantenimientoBalanzaComponent,
                commodities_component_1.RYDMantenimientoCommoditiesComponent,
                exportadores_component_1.RYDMantenimientoExportadorComponent,
                login_registro_component_1.RegistroUsuarioComponent,
                login_recuperar_contrasenia_component_1.RecuperarContraseniaComponent,
                usuario_list_component_1.UsuarioListComponent,
                usuario_cambio_vendedor_component_1.UsuarioCambioVendedorComponent,
                dato_fiscal_vendedor_component_1.VendedoresListComponent,
                vendedor_status_component_1.VendedorStatusComponent,
                filtro_fecha_component_1.FiltroFechaComponent,
                dropdown_component_1.DropdownComponent,
                mensaje_component_1.MensajeComponent,
                mensaje_modal_component_1.MensajeModalComponent,
                spinner_component_1.SpinnerComponent,
                spinner_small_component_1.SpinnerSmallComponent,
                customFilter_1.CustomFilter,
                customFilterOr_1.CustomFilterOr,
                customFilterContain_1.CustomFilterContain,
                orderedColumn_1.OrderedColumn,
                shortenString_1.ShortenStringPipe,
                pesificacion_component_1.PesificacionComponent,
                carta_porte_modal_component_1.CartaPorteModalComponent
            ],
            providers: [
                common_1.DatePipe,
                SessionDataService_1.SessionDataService,
                NavService_1.NavService,
                FloatMsgService_1.FloatMsgService,
                login_guard_1.LoginGuard,
                DataService_1.DataService,
                flete_service_1.FleteService,
                aduana_service_1.AduanaService,
                layout_service_1.LayoutService,
                liquidacion_service_1.LiquidacionService,
                liquidacion_service_1.LiquidacionAprobadaService,
                liquidacion_service_1.LiquidacionObservadaService,
                liquidacion_service_1.LiquidacionPagaService,
                liquidacion_service_1.LiquidacionNGAprobadaService,
                liquidacion_service_1.LiquidacionNGObservadaService,
                liquidacion_service_1.LiquidacionNGPagaService,
                liquidacion_service_1.LiquidacionProformaService,
                pago_service_1.PagoService,
                pago_service_1.PagoEmitidoService,
                pago_service_1.PagoEmitidoNGService,
                cuenta_corriente_service_1.CuentaCorrienteService,
                cuenta_corriente_service_1.CuentaCorrienteAgrupadaService,
                ryd_service_1.RYDInformeService,
                ryd_mantenimiento_service_1.RYDMantenimientoBalanzaService,
                ryd_mantenimiento_service_1.RYDMantenimientoService,
                ryd_mantenimiento_service_1.RYDMantenimientoCommoditiesService,
                ryd_mantenimiento_service_1.RYDMantenimientoExportadorService,
                ryd_service_1.RYDListadoPesadasService,
                usuario_service_1.UsuarioService,
                vendedor_status_service_1.VendedorStatusService,
                factura_service_1.FacturaService,
                ServiceLocator_1.ServiceLocator,
                ModalService_1.ModalService,
                SecurityService_1.SecurityService
            ],
            bootstrap: [app_component_1.AppComponent],
            schemas: [core_1.CUSTOM_ELEMENTS_SCHEMA]
        })
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map