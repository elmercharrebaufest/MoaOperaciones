import { DatePipe } from "@angular/common";

import {
    CUSTOM_ELEMENTS_SCHEMA,
    ErrorHandler,
    NgModule,
    NO_ERRORS_SCHEMA
} from "@angular/core";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { ReCaptchaModule } from "angular2-recaptcha";
import { BlockUIModule } from 'ng-block-ui';
import { SelectModule } from "ng2-select";
import { NgxMaskModule } from "ngx-mask";
import { ModalModule } from "ngx-modal";
import { LoggerModule, NgxLoggerLevel } from "ngx-logger";
import { MultiSelectModule } from 'primeng/multiselect';
import { NgxPaginationModule } from "ngx-pagination";
import { AduanaService } from "./aduana/aduana.service";
import { AltasComponent } from "./alta-proveedores/altas/altas.component";
import { EmpresaGranosComponent } from "./alta-proveedores/empresa-granos/empresa-granos.component";
import { EmpresaGranosService } from "./alta-proveedores/empresa-granos/empresa-granos.service";
import { EmpresaNoGranosComponent } from "./alta-proveedores/empresa-no-granos/empresa-no-granos.component";
import { EstadoSolicitudComponent } from "./alta-proveedores/estado-solicitud/estado-solicitud.component";
import { EstadoSolicitudService } from "./alta-proveedores/estado-solicitud/estado-solicitud.service";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { LoginGuard } from "./common/security/login-guard";
import { DataService } from "./common/services/DataService";
import { FloatMsgService } from "./common/services/FloatMsgService";
import { ModalService } from "./common/services/ModalService";
import { NavService } from "./common/services/NavService";
import { SecurityService } from "./common/services/SecurityService";
import { ServiceLocator } from "./common/services/ServiceLocator";
import { SessionDataService } from "./common/services/SessionDataService";
import { SharedModule } from "./common/shared.module";
import { ContactoMailComponent } from "./contacto-mail/contacto-mail.component";
import { NoAutorizadoComponent } from "./error/error.no-autorizado.component";
import { HomeComponent } from "./home/home.component";
import { HomeNGSComponent } from "./home/no-granos/home.no-granos.component";
import { LayoutComponent } from "./layout/layout.component";
import { LayoutService } from "./layout/layout.service";
import { CarouselNotificacionesComponent } from './notificaciones/carousel-notificaciones/carousel-notificaciones.component';
import { NotificacionesService } from "./notificaciones/notificaciones.service";
import { PesificacionComponent } from "./pesificacion/pesificacion.component";
import { UsuarioAltaEmpresaNoGranosComponent } from "./usuario/alta-empresa-no-granos/usuario.alta-empresa-no-granos.component";
import { UsuarioCambioVendedorComponent } from "./usuario/cambio-vendedor/usuario.cambio-vendedor.component";
import { UsuarioListComponent } from "./usuario/list/usuario.list.component";
import { UsuarioService } from "./usuario/usuario.service";
import { VendedorStatusComponent } from "./vendedor/vendedor_status.component";
import { VendedorStatusService } from "./vendedor/vendedor_status.service";
import { TicketPesadaComponent } from './ticket-pesada/ticket-pesada.component'
import { TicketPesadaService } from "./ticket-pesada/ticket-pesada.service";
import { ConsultaBaseComponent } from "./consulta/consulta.component";
import { ConsultaService } from "./consulta/consulta.service";
import { VentaSustentableBaseComponent } from './venta-sustentable/venta-sustentable.component';
import { LogPesificacionModule } from './log-pesificacion/log-pesificacion.module';
import { ComprasModule } from "./compras/compras.module";
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { PesificacionesGuardadasComponent } from './pesificacion/pesificaciones-guardadas/pesificaciones-guardadas.component';


import { PesificacionBaseComponent } from "./pesificacion/pesificacion-base.component";
import { ModificarDatosComponent } from './usuario/modificar-datos/modificar-datos.component';
import { ToastModule } from "primeng/toast";
import { UsuarioAuditoriaListComponent } from './usuario/usuario-auditoria-list/usuario-auditoria-list.component';
import { LegajoExternoComponent } from './compras/legajo-externo/legajo-externo.component';
import { GlobalErrorHandler } from "./common/services/GlobalErrorHandler";
import { VerVendedoresComponent } from "./usuario/ver-vendedores/ver-vendedores.component";

import { QuillModule } from "ngx-quill";
import { ModalNotificacionesComponent } from "./notificaciones/modal-notificaciones/modal-notificaciones.component";


import { AngularEditorModule } from '@kolkov/angular-editor';;
import { ComunicacionesComponent } from './comunicaciones/comunicaciones.component'
import { NotificacionesModule } from "./notificaciones/notificaciones.module";
import { AprobacionExternaComponent } from './aprobacion-externa/aprobacion-externa.component'

import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from "primeng/button";
import { TooltipModule } from "primeng/tooltip";
import { MessageSpinnerComponent } from "./common/message-spinner/message-spinner.component";
import { CalendarModule } from 'primeng/calendar';
import { ErrorInterceptor } from "./error.interceptor";
import { LogViewerComponent } from './log-viewer/log-viewer.component';
import { ZoomControlComponent } from './zoom-control/zoom-control.component';
import { ChatbotComponent } from './chatbot/chatbot.component';

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgxPaginationModule,
    SelectModule,
    ModalModule,
    ReCaptchaModule,
    SharedModule,
    LogPesificacionModule,
    CalendarModule,
    NgxMaskModule.forRoot(),
    BlockUIModule.forRoot(),
        LoggerModule.forRoot(
            {
                serverLoggingUrl: '/api/Logger/Front',
                level: NgxLoggerLevel.TRACE,
                serverLogLevel: NgxLoggerLevel.TRACE
            }
        ),
    ComprasModule,
    ConfirmDialogModule,
    MultiSelectModule,
        ToastModule,
        DropdownModule,
        ButtonModule,
        TooltipModule,
    QuillModule,
    AngularEditorModule,
    NotificacionesModule
  ],
  declarations: [
    AppComponent,
    ContactoMailComponent,
    HomeComponent,
    HomeNGSComponent,
    LayoutComponent,
    NoAutorizadoComponent,
    UsuarioListComponent,
    UsuarioAltaEmpresaNoGranosComponent,
    UsuarioCambioVendedorComponent,
    VendedorStatusComponent,
    PesificacionComponent,
    EmpresaGranosComponent,
    EmpresaNoGranosComponent,
    AltasComponent,
    EstadoSolicitudComponent,
    CarouselNotificacionesComponent,
    VentaSustentableBaseComponent,
    TicketPesadaComponent,
    ConsultaBaseComponent,
    VentaSustentableBaseComponent,
    PesificacionesGuardadasComponent,
    PesificacionBaseComponent,
    ModificarDatosComponent,
      UsuarioAuditoriaListComponent,
      LegajoExternoComponent,
      VerVendedoresComponent,
    ModalNotificacionesComponent,
    ComunicacionesComponent,
    AprobacionExternaComponent,
    MessageSpinnerComponent,
    LogViewerComponent,
    ZoomControlComponent,
    ChatbotComponent
    ],
    providers: [
        DatePipe,
        SessionDataService,
        NavService,
        FloatMsgService,
        LoginGuard,
        DataService,
        AduanaService,
        LayoutService,
        UsuarioService,
        VendedorStatusService,
        ServiceLocator,
        ModalService,
        SecurityService,
        EmpresaGranosService,
        EstadoSolicitudService,
        NotificacionesService,
        TicketPesadaService,
        ConsultaService,
        ConfirmationService,
        { provide: ErrorHandler, useClass: GlobalErrorHandler },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true } 
    ],
    bootstrap: [AppComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA]
})
export class AppModule { }