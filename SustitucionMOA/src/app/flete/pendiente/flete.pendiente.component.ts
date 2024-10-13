import { Component, OnInit, ViewChild } from '@angular/core';
import { FleteBaseComponent } from './../flete.component';
import { FleteService, FletePendienteService } from './../flete.service';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';

@Component({
    selector: 'app-flete-pendiente',
    templateUrl: `flete.pendiente.component.html`,
    providers: [{ provide: FleteService, useClass: FletePendienteService }]
})
export class FletePendienteComponent extends FleteBaseComponent {

    tituloArchivo = "FletesPendientes.xls";
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimoMes;
    filtroFechaKey: string = 'NGFletPend_Periodo';

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