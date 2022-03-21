import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LiquidacionNGPendienteRegistroComponent } from './pendiente-registro/liquidacion.no-granos.pendiente-registro.component';
import { LiquidacionNGRegistradoComponent } from './registrada/liquidacion.no-granos.registrado.component';

const routes: Routes = [
    { path: '', component: LiquidacionNGRegistradoComponent },
    { path: '', component: LiquidacionNGPendienteRegistroComponent },
    {
      path: "registrado",
      component: LiquidacionNGRegistradoComponent,
    },
    {
      path: "pendiente-registro",
      component: LiquidacionNGPendienteRegistroComponent,
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LiquidacionNoGranosRoutingModule { }
