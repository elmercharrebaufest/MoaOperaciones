import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FleteAFacturarComponent } from "./a-facturar/flete.a-facturar.component";
import { FleteFacturadoComponent } from "./facturado/flete.facturado.component";
import { FletePendienteComponent } from "./pendiente/flete.pendiente.component";

const routes: Routes = [
    { path: '', component: FleteAFacturarComponent },
    { path: "a-facturar", component: FleteAFacturarComponent },
    { path: "facturado", component: FleteFacturadoComponent },
    { path: "pendiente", component: FletePendienteComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class FleteRoutingModule { }
