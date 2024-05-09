import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListarArchivoBoletoComponent } from './listar-archivo-boleto/listar-archivo-boleto.component';

const routes: Routes = [
  { path: 'listar', component: ListarArchivoBoletoComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ArchivoBoletoRoutingModule { }
