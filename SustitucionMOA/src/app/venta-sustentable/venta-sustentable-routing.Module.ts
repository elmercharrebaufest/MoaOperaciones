import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListadoCamposComponent } from "./listado-campos/listado-campos.component";
import { AltaComponent } from "./alta/alta.component";

const routes: Routes = [
    { path: '', component: ListadoCamposComponent },
    { path: 'venta-sustentable', component: ListadoCamposComponent },
    { path: "Alta", component: AltaComponent },
    {
      path: "/venta-sustentable/Listado",
      component: ListadoCamposComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class VentaSustentableRoutingModule { }
