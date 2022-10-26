import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EcheqGestionComponent } from './gestion/echeq-gestion.component';

const routes: Routes = [
    { path: '', component: EcheqGestionComponent },
    { path: "gestion", component: EcheqGestionComponent }

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class EcheqRoutingModule { }
