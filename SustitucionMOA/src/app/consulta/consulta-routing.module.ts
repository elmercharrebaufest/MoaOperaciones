import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CrearConsultaComponent } from './crear-consulta/crear-consulta.component';
import { DetalleConsultaComponent } from './detalle/consulta-detalle.component';
import { MisConsultasComponent } from './mis-consultas/mis-consultas.component';
import { CrearConsultaInternaComponent } from './crear-consulta-interna/crear-consulta-interna.component';

const routes: Routes = [
    { path: '', component: CrearConsultaComponent },
    { path: "mis-consultas", component: MisConsultasComponent,},
    { path: "crear-consulta", component: CrearConsultaComponent,},
    { path: "detalle", component: DetalleConsultaComponent},
    { path: "crear-consulta-interna", component: CrearConsultaInternaComponent}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ConsultaRoutingModule { }