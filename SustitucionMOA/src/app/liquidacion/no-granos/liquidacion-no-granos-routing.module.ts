import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LiquidacionNGAprobadaComponent } from "./aprobada/liquidacion.no-granos.aprobada.component";
import { LiquidacionNGObservadaComponent } from "./observada/liquidacion.no-granos.observada.component";

const routes: Routes = [
    { path: '', component: LiquidacionNGAprobadaComponent },
    {
      path: "aprobada",
      component: LiquidacionNGAprobadaComponent,
    },
    {
      path: "observada",
      component: LiquidacionNGObservadaComponent,
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LiquidacionNoGranosRoutingModule { }
