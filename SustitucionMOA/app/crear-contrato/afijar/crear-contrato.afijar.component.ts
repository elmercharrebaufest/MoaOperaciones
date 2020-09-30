import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoAFijarService } from './../crear-contrato.service';

@Component({
    selector: 'app-crear-contrato-afijar',
    templateUrl: `./app/crear-contrato/afijar/crear-contrato.afijar.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: CrearContratoService, useClass: CrearContratoAFijarService }]
})
export class CrearContratoAFijarComponent extends CrearContratoBaseComponent {

    
    setTabs() {
        this.setMenuSeccionTab("crear-contrato", "A Fijar");
    }

    //showModalTableResponsive(Contrato: any) {
    //    this.modalService.openModalTableResponsive("Contrato Vigente", [
    //        { etiqueta: "Último Movimiento", valor: Contrato.fecha },
    //        { etiqueta: "Contrato Molinos", valor: Contrato.nroContrato },
    //        { etiqueta: "Contrato Proveedor", valor: Contrato.contrvend },
    //        { etiqueta: "Estado Boleto", valor: Contrato.estado },
    //        { etiqueta: "Producto", valor: Contrato.material },
    //        { etiqueta: "Pactado", valor: Contrato.cantKilosString },
    //        { etiqueta: "Entregado", valor: Contrato.aplicacionesString },
    //        { etiqueta: "Liquidado", valor: Contrato.liquidadoString }
    //    ]);
    //    return false;
    //}
}