import { NgModule } from '@angular/core';
import { ListadoNotificacionesComponent } from './listado-notificaciones/listado-notificaciones.component';
import { AltaNotificacionesComponent } from './alta-notificaciones/alta-notificaciones.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', component: ListadoNotificacionesComponent },
  { path: "alta", component: AltaNotificacionesComponent },
  { path: "alta/:id", component: AltaNotificacionesComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class NotificacionesRoutingModule { }
