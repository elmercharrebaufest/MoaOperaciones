import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ComunicacionesComponent } from './comunicaciones.component';

const routes: Routes = [
  { path: '', component: ComunicacionesComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class ComunicacionesRoutingModule { }
