import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PagoDetalleComponent } from './detalle/pago.detalle.component';
import { PagoEmitidoComponent } from './emitido/pago.emitido.component';
import { PagoComponent } from './pago.component';

const routes: Routes = [
    { path: '',  component: PagoComponent },
    { path: "emitido", component: PagoEmitidoComponent },
    { path: "detalle/:id", component: PagoDetalleComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PagoRoutingModule { }
