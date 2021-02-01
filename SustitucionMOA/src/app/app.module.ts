import { DatePipe } from "@angular/common";
import {
  CUSTOM_ELEMENTS_SCHEMA,
  NgModule,
  NO_ERRORS_SCHEMA,
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpModule } from "@angular/http";
import { BrowserModule } from "@angular/platform-browser";
import { AutocompleteLibModule } from "angular-ng-autocomplete";
import { ReCaptchaModule } from "angular2-recaptcha";
import { Ng2AutoCompleteModule } from "ng2-auto-complete";
import { SelectModule } from "ng2-select";
import { ModalModule } from "ngx-modal";
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
import { PesificacionComponent } from "./pesificacion/pesificacion.component";
import { UsuarioCambioVendedorComponent } from "./usuario/cambio-vendedor/usuario.cambio-vendedor.component";
import { UsuarioListComponent } from "./usuario/list/usuario.list.component";
import { UsuarioService } from "./usuario/usuario.service";
import { VendedorStatusComponent } from "./vendedor/vendedor_status.component";
import { VendedorStatusService } from "./vendedor/vendedor_status.service";
import { UsuarioAltaEmpresaNoGranosComponent } from "./usuario/alta-empresa-no-granos/usuario.alta-empresa-no-granos.component";
import { NumericDirective } from './common/directive/numeric.directive';
import { NgxMaskModule } from "ngx-mask";
import { AltaNotificacionesComponent } from './notificaciones/alta-notificaciones/alta-notificaciones.component';
import { ListadoNotificacionesComponent } from './notificaciones/listado-notificaciones/listado-notificaciones.component'
import { NotificacionesService } from "./notificaciones/notificaciones.service";
import { CarouselNotificacionesComponent } from './notificaciones/carousel-notificaciones/carousel-notificaciones.component'
import { BlockUIModule } from 'ng-block-ui';;
import { TicketPesadaComponent } from './ticket-pesada/ticket-pesada.component'
import { TicketPesadaService } from "./ticket-pesada/ticket-pesada.service";

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
    AutocompleteLibModule,
    SharedModule,
    NgxMaskModule.forRoot(),
    BlockUIModule.forRoot(),
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
    AltaNotificacionesComponent,
    ListadoNotificacionesComponent,
    CarouselNotificacionesComponent,
    NumericDirective
,
    TicketPesadaComponent
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
    TicketPesadaService
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class AppModule {}
