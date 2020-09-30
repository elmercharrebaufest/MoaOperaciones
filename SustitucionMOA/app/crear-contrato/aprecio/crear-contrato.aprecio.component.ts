import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoAPrecioService } from './../crear-contrato.service';

@Component({
    selector: 'app-crear-contrato-aprecio',
    templateUrl: `./app/crear-contrato/aprecio/crear-contrato.aprecio.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: CrearContratoService, useClass: CrearContratoAPrecioService }]
})
export class CrearContratoAPrecioComponent extends CrearContratoBaseComponent {

    
    setTabs() {
        this.setMenuSeccionTab("crear-contrato", "A Precio");
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