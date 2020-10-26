import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ContratoVigenteComponent } from './vigente/contrato.vigente.component';
import { ContratoAmpliacionComponent } from './ampliacion/contrato.ampliacion.component';
import { ContratoFijacionComponent } from './fijacion/contrato.fijacion.component';
import { ContratoAnulacionComponent } from './anulacion/contrato.anulacion.component';
import { ContratoDetalleComponent } from './detalle/contrato.detalle.component';
import { ContratoDetalleFijacionComponent } from './detalle-fijacion/contrato.detalle-fijacion.component';
import { CrearContratoAPrecioComponent } from '../crear-contrato/aprecio/crear-contrato.aprecio.component';
import { CrearContratoAFijarComponent } from '../crear-contrato/afijar/crear-contrato.afijar.component';
import { CrearContratoBaseComponent } from '../crear-contrato/crear-contrato.component';
import { CrearContratoFijacionComponent } from '../crear-contrato/fijacion/crear-contrato.fijacion.component';


const routes: Routes = [
    { path: '', component: ContratoVigenteComponent },
    { path: 'vigente', component: ContratoVigenteComponent },
    { path: "fijacion", component: ContratoFijacionComponent },
    { path: "ampliacion", component: ContratoAmpliacionComponent },
    { path: "anulacion", component: ContratoAnulacionComponent },
    { path: "detalle/:id", component: ContratoDetalleComponent },
    { path: "detalle-fijacion/:id/:id2", component: ContratoDetalleFijacionComponent },
    { path: "crear", component: CrearContratoBaseComponent },
    { path: "crear/aprecio", component: CrearContratoAPrecioComponent },
    { path: "crear/afijar", component: CrearContratoAFijarComponent },
    { path: "crear/fijacion", component: CrearContratoFijacionComponent },

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ContratoRoutingModule { }
