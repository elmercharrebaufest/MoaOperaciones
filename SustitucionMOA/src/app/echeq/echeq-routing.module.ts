import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EcheqGestionComponent } from './gestion/echeq-gestion.component';
import { MisEcheqComponent } from './mis-echeq/mis-echeq.component';

const routes: Routes = [
    { path: '', component: EcheqGestionComponent },
    { path: "gestion", component: EcheqGestionComponent },
    { path: "mis-echeq", component: MisEcheqComponent }


];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class EcheqRoutingModule { }
