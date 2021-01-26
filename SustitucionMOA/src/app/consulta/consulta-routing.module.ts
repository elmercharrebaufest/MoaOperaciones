import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CrearConsultaComponent } from './crear-consulta/crear-consulta.component';
import { MisConsultasComponent } from './mis-consultas/mis-consultas.component';

const routes: Routes = [
    { path: '', component: MisConsultasComponent },
    { path: "mis-consultas", component: MisConsultasComponent,},
    { path: "crear-consulta", component: CrearConsultaComponent,}
    
    /*
    CUANDO TENGA LAS DEMAS PANTALLAS.
    ,
    {
      path: "observada",
      component: LiquidacionObservadaComponent,
    },
    { path: "paga", component: LiquidacionPagaComponent },
    {
      path: "proforma/:id",
      component: LiquidacionProformaComponent,
    } */
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ConsultaRoutingModule { }