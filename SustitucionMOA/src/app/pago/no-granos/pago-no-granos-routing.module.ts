import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PagoEmitidoNGSComponent } from './emitido/pago.no-granos.emitido.component';

const routes: Routes = [
    { path: '', redirectTo: 'emitido', pathMatch: 'full' },
    { path: "emitido", component: PagoEmitidoNGSComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PagoNoGranosRoutingModule { }
