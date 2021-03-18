import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListadoCamposComponent } from "./listado-campos/listado-campos.component";
import { AltaComponent } from "./alta/alta.component";
import { EdicionComponent } from "./edicion/edicion.component";

const routes: Routes = [
    { path: '', component: AltaComponent },
    { path: "alta", component: AltaComponent },
    { path: "listado-campos", component: ListadoCamposComponent,},
    { path: "edicion", component: EdicionComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class VentaSustentableRoutingModule { }
