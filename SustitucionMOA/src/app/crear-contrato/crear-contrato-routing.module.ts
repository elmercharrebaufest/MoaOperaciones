import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CrearContratoAPrecioComponent } from './aprecio/crear-contrato.aprecio.component';
import { CrearContratoAFijarComponent } from './afijar/crear-contrato.afijar.component';
import { CrearContratoFijacionComponent } from './fijacion/crear-contrato.fijacion.component';
import { CrearContratoAltaMasivaComponent } from './alta-masiva/crear-contrato.alta-masiva.component';
import { CrearContratoCargarNegocioComponent } from './cargarnegocio/crear-contrato.cargarnegocio.component';

const routes: Routes = [
    { path: '', component: CrearContratoAPrecioComponent },
    { path: 'aprecio', component: CrearContratoAPrecioComponent },
    { path: "afijar", component: CrearContratoAFijarComponent },
    { path: "fijacion", component: CrearContratoFijacionComponent },
    { path: "cargarnegocio", component: CrearContratoCargarNegocioComponent },
    { path: 'aprecio/:id', component: CrearContratoAPrecioComponent },
    { path: "afijar/:id", component: CrearContratoAFijarComponent },
    { path: "fijacion/:id", component: CrearContratoFijacionComponent },
    { path: "alta-masiva", component: CrearContratoAltaMasivaComponent },

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CrearContratoRoutingModule { }
