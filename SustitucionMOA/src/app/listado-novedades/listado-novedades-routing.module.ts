import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListadoNovedadesComponent } from './listado-novedades.component';

const routes: Routes = [
    { path: '', component: ListadoNovedadesComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ListadoNovedadesRoutingModule { }
