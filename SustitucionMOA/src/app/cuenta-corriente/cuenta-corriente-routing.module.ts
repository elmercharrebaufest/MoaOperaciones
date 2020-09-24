import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CuentaCorrienteAgrupadaComponent } from './agrupada/cuenta-corriente.agrupada.component';
import { CuentaCorrienteBaseComponent } from './cuenta-corriente.component';

const routes: Routes = [
    { path: '', component: CuentaCorrienteBaseComponent },
    {
      path: "simple",
      component: CuentaCorrienteBaseComponent,
    },
    {
      path: "simple/:id",
      component: CuentaCorrienteBaseComponent,
    },
    {
      path: "agrupada",
      component: CuentaCorrienteAgrupadaComponent,
    },
    {
      path: "agrupada/:id",
      component: CuentaCorrienteAgrupadaComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CuentaCorrienteRoutingModule { }
