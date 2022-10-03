import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GestionEcheqComponent } from './gestion/gestion.component';
import { MisEcheqComponent } from './mis-echeq/mis-echeq.component';


const routes: Routes = [
    { path: '', component: GestionEcheqComponent },
    { path: 'gestion', component: GestionEcheqComponent },
    { path: 'mis-echeq', component: MisEcheqComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class EcheqRoutingModule { }
