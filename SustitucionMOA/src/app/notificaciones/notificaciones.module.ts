import { NotificacionesRoutingModule } from './notificaciones-routing.module';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { AltaNotificacionesComponent } from './alta-notificaciones/alta-notificaciones.component';
import { ListadoNotificacionesComponent } from './listado-notificaciones/listado-notificaciones.component';
import { NotificacionesService } from './notificaciones.service';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../common/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';

@NgModule({
  imports: [
    CommonModule,
    AngularEditorModule,
    NotificacionesRoutingModule,
    FormsModule,
    SharedModule,
    NgxPaginationModule,
    TooltipModule,
    ToastModule
  ],
  declarations: [
    AltaNotificacionesComponent,
    ListadoNotificacionesComponent,
  ],
  providers: [
    NotificacionesService,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class NotificacionesModule { }
