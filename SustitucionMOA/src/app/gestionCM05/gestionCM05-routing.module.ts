import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GestionCM05Component } from "./gestionCM05.component";

const routes: Routes = [
    { path: '', component: GestionCM05Component },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GestionCM05RoutingModule { }
