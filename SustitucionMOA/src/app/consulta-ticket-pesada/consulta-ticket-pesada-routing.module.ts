import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConsultaTicketPesadaComponent } from './consulta-ticket-pesada.component';

const routes: Routes = [
  { path: '', component: ConsultaTicketPesadaComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConsultaTicketPesadaRoutingModule { }
