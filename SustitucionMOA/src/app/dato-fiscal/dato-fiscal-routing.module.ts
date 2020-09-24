import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DatoFiscalBaseComponent } from './dato-fiscal.component';
import { DocumentacionComponent } from './documentacion/documentacion.component';
import { VendedoresListComponent } from './vendedor/dato-fiscal.vendedor.component';

const routes: Routes = [
    { path: '', redirectTo: '/situacion-fiscal', pathMatch: 'full' },
    { path: "situacion-fiscal", component: DatoFiscalBaseComponent, },
    { path: "situacion-fiscal/:id", component: DatoFiscalBaseComponent, },
    { path: "situacion-fiscal/:id/:id2", component: DatoFiscalBaseComponent, },
    { path: "documentacion", component: DocumentacionComponent },
    { path: "vendedor", component: VendedoresListComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DatoFiscalRoutingModule { }
