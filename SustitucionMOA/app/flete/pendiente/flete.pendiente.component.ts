import { Component, OnInit, ViewChild } from '@angular/core';
import { FleteBaseComponent } from './../flete.component';
import { FleteService, FletePendienteService } from './../flete.service';

@Component({
    selector: 'my-app',
    templateUrl: `./app/flete/pendiente/flete.pendiente.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: FleteService, useClass: FletePendienteService }]
})
export class FletePendienteComponent extends FleteBaseComponent {

    tituloArchivo = "FletesPendientes.xls";

    setTabs() {
        this.setMenuSeccionTab("flete", "Viajes Pendientes");
    }

    showModalTableResponsive(Viaje: any) {
        this.modalService.openModalTableResponsive("Viaje Pendiente", [
            { etiqueta: "CCPP", valor: Viaje.ccpp },
            { etiqueta: "Fecha", valor: Viaje.fechaCCPP },
            { etiqueta: "Patente", valor: Viaje.patente },
            { etiqueta: "KG CCPP", valor: Viaje.kgString },
            { etiqueta: "Descripción", valor: Viaje.descMat },
            { etiqueta: "Origen", valor: Viaje.origen },
            { etiqueta: "Destino", valor: Viaje.destino },
            { etiqueta: "Peaje", valor: Viaje.peajeString },
            { etiqueta: "Playas", valor: Viaje.playaString }
        ]);
        return false;
    }
}