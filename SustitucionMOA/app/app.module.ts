import { DatePipe } from "@angular/common";
import {
    CUSTOM_ELEMENTS_SCHEMA, NgModule,

    NO_ERRORS_SCHEMA
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpModule } from "@angular/http";
import { BrowserModule } from "@angular/platform-browser";
import { AutocompleteLibModule } from 'angular-ng-autocomplete';
import { ReCaptchaModule } from "angular2-recaptcha";
import { Ng2AutoCompleteModule } from "ng2-auto-complete";
import { SelectModule } from "ng2-select";
import { ModalModule } from "ngx-modal";
import { NgxPaginationModule } from "ngx-pagination";
import { AduanaBaseComponent } from "./aduana/aduana.component";
import { AduanaService } from "./aduana/aduana.service";
import { CamaraConsolidacionComponent } from "./aduana/camara/consolidacion/camara.consolidacion.component";
import { CamaraMuelleComponent } from "./aduana/camara/muelle/camara.muelle.component";
import { InformacionMeteorologicaComponent } from "./aduana/informacion-meteorologica/informacion-meteorologica.component";
import { PesadaDetalleComponent } from "./aduana/pesada/detalle/pesada.detalle.component";
import { PesadaHistoricaComponent } from "./aduana/pesada/historica/pesada.historica.component";
import { PesadaOnlineComponent } from "./aduana/pesada/online/pesada.online.component";
import { AltasComponent } from "./alta-proveedores/altas/altas.component";
import { CuitInvalidoComponent } from "./alta-proveedores/cuit-invalido/cuit-invalido.component";
import { EmpresaGranosComponent } from "./alta-proveedores/empresa-granos/empresa-granos.component";
import { EmpresaGranosService } from "./alta-proveedores/empresa-granos/empresa-granos.service";
import { EmpresaNoGranosComponent } from "./alta-proveedores/empresa-no-granos/empresa-no-granos.component";
import { EstadoSolicitudComponent } from './alta-proveedores/estado-solicitud/estado-solicitud.component';
import { EstadoSolicitudService } from './alta-proveedores/estado-solicitud/estado-solicitud.service';
import { ProveedorDetalleComponent } from "./alta-proveedores/proveedor-detalle/proveedor-detalle.component";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { CartaPorteAplicacionComponent } from "./carta-porte/aplicacion/carta-porte.aplicacion2.component";
import { CartaPorteModalComponent } from "./carta-porte/carta-porte-modal/carta-porte-modal.component";
import { CartaPorteBaseComponent } from "./carta-porte/carta-porte.component";
import { CartaPorteDescargaComponent } from "./carta-porte/descarga/carta-porte.descarga2.component";
import { CartaPorteDetalleComponent } from "./carta-porte/detalle/carta-porte.detalle2.component";
import { CartaPorteFormularioComponent } from "./carta-porte/formulario/carta-porte.formulario.component";
import { NumericDirective } from "./common/directive/numeric.directive";
import { ArchivoPipe } from "./common/pipes/archivos.pipe";
import { CustomFilter } from "./common/pipes/customFilter";
import { CustomFilterContain } from "./common/pipes/customFilterContain";
import { CustomFilterOr } from "./common/pipes/customFilterOr";
import { OrderedColumn } from "./common/pipes/orderedColumn";
import { ShortenStringPipe } from "./common/pipes/shortenString";
import { LoginGuard } from "./common/security/login-guard";
import { DataService } from "./common/services/DataService";
import { FloatMsgService } from "./common/services/FloatMsgService";
import { ModalService } from "./common/services/ModalService";
import { NavService } from "./common/services/NavService";
import { SecurityService } from "./common/services/SecurityService";
import { ServiceLocator } from "./common/services/ServiceLocator";
import { SessionDataService } from "./common/services/SessionDataService";
import { DropdownComponent } from "./common/view-child/dropdown/dropdown.component";
import { FiltroFechaComponent } from "./common/view-child/filtro-fecha/filtro-fecha.component";
import { MensajeModalComponent } from "./common/view-child/mensaje-modal/mensaje-modal.component";
import { MensajeComponent } from "./common/view-child/mensaje/mensaje.component";
import { SpinnerSmallComponent } from "./common/view-child/spinner-small/spinner-small.component";
import { SpinnerComponent } from "./common/view-child/spinner/spinner.component";
import { ContactoMailComponent } from "./contacto-mail/contacto-mail.component";
import { ContratoAmpliacionComponent } from "./contrato/ampliacion/contrato.ampliacion.component";
import { ContratoAnulacionComponent } from "./contrato/anulacion/contrato.anulacion.component";
import { ContratoBaseComponent } from "./contrato/contrato.component";
import { ContratoDetalleFijacionComponent } from "./contrato/detalle-fijacion/contrato.detalle-fijacion.component";
import { ContratoDetalleComponent } from "./contrato/detalle/contrato.detalle.component";
import { ContratoFijacionComponent } from "./contrato/fijacion/contrato.fijacion.component";
import { ContratoVigenteComponent } from "./contrato/vigente/contrato.vigente.component";
import { CuentaCorrienteAgrupadaComponent } from "./cuenta-corriente/agrupada/cuenta-corriente.agrupada.component";
import { CuentaCorrienteBaseComponent } from "./cuenta-corriente/cuenta-corriente.component";
import { CuentaCorrienteAgrupadaService, CuentaCorrienteService } from "./cuenta-corriente/cuenta-corriente.service";
import { DatoFiscalBaseComponent } from "./dato-fiscal/dato-fiscal.component";
import { DocumentacionComponent } from "./dato-fiscal/documentacion/documentacion.component";
import { VendedoresListComponent } from "./dato-fiscal/vendedor/dato-fiscal.vendedor.component";
import { VendedoresPendientesComponent } from './dato-fiscal/vendedores-pendientes/vendedores-pendientes.component';
import { NoAutorizadoComponent } from "./error/error.no-autorizado.component";
import { FacturaComponent } from "./factura/factura.component";
import { FacturaService } from "./factura/factura.service";
import { FleteAFacturarComponent } from "./flete/a-facturar/flete.a-facturar.component";
import { FleteFacturadoComponent } from "./flete/facturado/flete.facturado.component";
import { FleteBaseComponent } from "./flete/flete.component";
import { FleteService } from "./flete/flete.service";
import { FletePendienteComponent } from "./flete/pendiente/flete.pendiente.component";
import { HomeComponent } from "./home/home.component";
import { HomeNGSComponent } from "./home/no-granos/home.no-granos.component";
import { LayoutComponent } from "./layout/layout.component";
import { LayoutService } from "./layout/layout.service";
import { LiquidacionAprobadaComponent } from "./liquidacion/aprobada/liquidacion.aprobada.component";
import { LiquidacionBaseComponent } from "./liquidacion/liquidacion.component";
import {
    LiquidacionAprobadaService,


    LiquidacionNGAprobadaService,
    LiquidacionNGObservadaService,
    LiquidacionNGPagaService, LiquidacionObservadaService,
    LiquidacionPagaService,



    LiquidacionProformaService, LiquidacionService
} from "./liquidacion/liquidacion.service";
import { LiquidacionNGAprobadaComponent } from "./liquidacion/no-granos/aprobada/liquidacion.no-granos.aprobada.component";
import { LiquidacionNGObservadaComponent } from "./liquidacion/no-granos/observada/liquidacion.no-granos.observada.component";
import { LiquidacionNGPagaComponent } from "./liquidacion/no-granos/paga/liquidacion.no-granos.paga.component";
import { LiquidacionObservadaComponent } from "./liquidacion/observada/liquidacion.observada.component";
import { LiquidacionPagaComponent } from "./liquidacion/paga/liquidacion.paga.component";
import { LiquidacionProformaComponent } from "./liquidacion/proforma/liquidacion.proforma.component";
import { LoginComponent } from "./login/login.component";
import { RecuperarContraseniaComponent } from "./login/recuperar-contrasenia/login.recuperar-contrasenia.component";
import { RegistroUsuarioComponent } from "./login/registro/login.registro.component";
import { PagoDetalleComponent } from "./pago/detalle/pago.detalle.component";
import { PagoEmitidoComponent } from "./pago/emitido/pago.emitido.component";
import { PagoEmitidoNGSComponent } from "./pago/no-granos/emitido/pago.no-granos.emitido.component";
import { PagoComponent } from "./pago/pago.component";
import { PagoEmitidoNGService, PagoEmitidoService, PagoService } from "./pago/pago.service";
import { PesificacionComponent } from "./pesificacion/pesificacion.component";
import { RYDMantenimientoBalanzaComponent } from "./ryd-mantenimiento/balanzas/balanzas.component";
import { RYDMantenimientoCommoditiesComponent } from "./ryd-mantenimiento/commodities/commodities.component";
import { RYDMantenimientoExportadorComponent } from "./ryd-mantenimiento/exportadores/exportadores.component";
import { RYDMantenimientoBaseComponent } from "./ryd-mantenimiento/ryd-mantenimiento.component";
import {
    RYDMantenimientoBalanzaService,
    RYDMantenimientoCommoditiesService,
    RYDMantenimientoExportadorService, RYDMantenimientoService
} from "./ryd-mantenimiento/ryd-mantenimiento.service";
import { CargaPesadasComponent } from "./ryd/carga-pesadas/carga-pesadas.component";
import { InformeComponent } from "./ryd/informe/informe.component";
import { ListadoPesadasComponent } from "./ryd/listado-pesadas/listado-pesadas.component";
import { RYDBaseComponent } from "./ryd/ryd.component";
import { RYDInformeService, RYDListadoPesadasService } from "./ryd/ryd.service";
import { AltaUsuarioComponent } from "./usuario/alta/usuario.alta.component";
import { CambioContraseniaComponent } from "./usuario/cambio-contrasenia/usuario.cambio-contrasenia.component";
import { UsuarioCambioVendedorComponent } from "./usuario/cambio-vendedor/usuario.cambio-vendedor.component";
import { UsuarioListComponent } from "./usuario/list/usuario.list.component";
import { UsuarioService } from "./usuario/usuario.service";
import { VendedorStatusComponent } from "./vendedor/vendedor_status.component";
import { VendedorStatusService } from "./vendedor/vendedor_status.service";









;



;

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        AppRoutingModule,
        FormsModule,
        NgxPaginationModule,
        SelectModule,
        ModalModule,
        Ng2AutoCompleteModule,
        ReCaptchaModule,
        AutocompleteLibModule
    ],
    declarations: [
        NumericDirective,
        AppComponent,
        AduanaBaseComponent,
        AltaUsuarioComponent,
        CamaraConsolidacionComponent,
        CamaraMuelleComponent,
        CargaPesadasComponent,
        CartaPorteBaseComponent,
        CartaPorteAplicacionComponent,
        CartaPorteDetalleComponent,
        CartaPorteDescargaComponent,
        CartaPorteFormularioComponent,
        CambioContraseniaComponent,
        ContratoBaseComponent,
        ContratoAmpliacionComponent,
        ContratoAnulacionComponent,
        ContratoFijacionComponent,
        ContratoVigenteComponent,
        ContratoDetalleComponent,
        ContratoDetalleFijacionComponent,
        ContactoMailComponent,
        CuentaCorrienteBaseComponent,
        CuentaCorrienteAgrupadaComponent,
        DatoFiscalBaseComponent,
        DocumentacionComponent,
        FacturaComponent,
        FleteBaseComponent,
        FleteAFacturarComponent,
        FleteFacturadoComponent,
        FletePendienteComponent,
        HomeComponent,
        HomeNGSComponent,
        InformeComponent,
        InformacionMeteorologicaComponent,
        LayoutComponent,
        LiquidacionBaseComponent,
        LiquidacionAprobadaComponent,
        LiquidacionObservadaComponent,
        LiquidacionPagaComponent,
        LiquidacionNGAprobadaComponent,
        LiquidacionNGObservadaComponent,
        LiquidacionNGPagaComponent,
        ListadoPesadasComponent,
        LiquidacionProformaComponent,
        LoginComponent,
        NoAutorizadoComponent,
        PagoComponent,
        PagoEmitidoComponent,
        PagoEmitidoNGSComponent,
        PagoDetalleComponent,
        PesadaDetalleComponent,
        PesadaHistoricaComponent,
        PesadaOnlineComponent,
        RYDBaseComponent,
        RYDMantenimientoBaseComponent,
        RYDMantenimientoBalanzaComponent,
        RYDMantenimientoCommoditiesComponent,
        RYDMantenimientoExportadorComponent,
        RegistroUsuarioComponent,
        RecuperarContraseniaComponent,
        UsuarioListComponent,
        UsuarioCambioVendedorComponent,
        VendedoresListComponent,
        VendedorStatusComponent,
        FiltroFechaComponent,
        DropdownComponent,
        MensajeComponent,
        MensajeModalComponent,
        SpinnerComponent,
        SpinnerSmallComponent,
        CustomFilter,
        CustomFilterOr,
        CustomFilterContain,
        OrderedColumn,
        ShortenStringPipe,
        PesificacionComponent,
        CartaPorteModalComponent,
        EmpresaGranosComponent,
        EmpresaNoGranosComponent,
        AltasComponent,
        CuitInvalidoComponent,
        ProveedorDetalleComponent,
        EstadoSolicitudComponent,
        ArchivoPipe,
        VendedoresPendientesComponent    ],
    providers: [
        DatePipe,
        SessionDataService,
        NavService,
        FloatMsgService,
        LoginGuard,
        DataService,
        FleteService,
        AduanaService,
        LayoutService,
        LiquidacionService,
        LiquidacionAprobadaService,
        LiquidacionObservadaService,
        LiquidacionPagaService,
        LiquidacionNGAprobadaService,
        LiquidacionNGObservadaService,
        LiquidacionNGPagaService,
        LiquidacionProformaService,
        PagoService,
        PagoEmitidoService,
        PagoEmitidoNGService,
        CuentaCorrienteService,
        CuentaCorrienteAgrupadaService,
        RYDInformeService,
        RYDMantenimientoBalanzaService,
        RYDMantenimientoService,
        RYDMantenimientoCommoditiesService,
        RYDMantenimientoExportadorService,
        RYDListadoPesadasService,
        UsuarioService,
        VendedorStatusService,
        FacturaService,
        ServiceLocator,
        ModalService,
        SecurityService,
        EmpresaGranosService,
        EstadoSolicitudService
    ],
    bootstrap: [AppComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class AppModule { }
