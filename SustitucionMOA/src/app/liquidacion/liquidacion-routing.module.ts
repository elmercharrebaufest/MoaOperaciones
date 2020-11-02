import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LiquidacionAprobadaComponent } from "./aprobada/liquidacion.aprobada.component";
import { LiquidacionObservadaComponent } from "./observada/liquidacion.observada.component";
import { LiquidacionPagaComponent } from "./paga/liquidacion.paga.component";
import { LiquidacionProformaComponent } from "./proforma/liquidacion.proforma.component";

const routes: Routes = [
    { path: '', component: LiquidacionAprobadaComponent },
    { path: "aprobada", component: LiquidacionAprobadaComponent },
    {
      path: "observada",
      component: LiquidacionObservadaComponent,
    },
    { path: "paga", component: LiquidacionPagaComponent },
    {
      path: "proforma/:id",
      component: LiquidacionProformaComponent,
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LiquidacionRoutingModule { }
