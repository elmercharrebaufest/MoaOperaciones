import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ApikeyComponent } from './apikey.component';

const routes: Routes = [
    { path: '', component: ApikeyComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ApikeyRoutingModule { }
