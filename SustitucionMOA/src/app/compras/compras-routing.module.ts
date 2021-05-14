import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SolpComponent } from './solp.component';
import { DashboardComponent } from './dashboard/dashboard.component';

const routes: Routes = [
    { path: '', component: DashboardComponent },
    { path: "solp", component: SolpComponent },
    { path: "dashboard", component: DashboardComponent }

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ComprasRoutingModule { }
