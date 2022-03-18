import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LiquidacionNGRegistradoComponent } from './registrada/liquidacion.no-granos.registrado.component';

const routes: Routes = [
    { path: '', component: LiquidacionNGRegistradoComponent },
    {
      path: "registrado",
      component: LiquidacionNGRegistradoComponent,
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LiquidacionNoGranosRoutingModule { }
