import { Component, OnInit, ViewChild } from '@angular/core';
import { CartaPorteBaseComponent } from './../carta-porte.component';
import { CartaPorteService, CartaPorteDescargaService } from './../carta-porte.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';

@Component({
    selector: 'app-carta-porte-descarga',
    templateUrl: `./app/carta-porte/descarga/carta-porte.descarga.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: CartaPorteService, useClass: CartaPorteDescargaService }]
})
export class CartaPorteDescargaComponent extends CartaPorteBaseComponent {

    tituloArchivo = "ReporteDescargas.xls";

    constructor(protected service: CartaPorteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CARTAS PORTE"); }

    setTabs() {
        this.setMenuSeccionTab("carta-porte", "Descargas");
    }

    showModalTableResponsive(recepcionInfo: any) {
        this.modalService.openModalTableResponsive("Carta de Porte", [
            { etiqueta: "Fecha", valor: recepcionInfo.fechaDescarga },
            { etiqueta: "CCPP Nº", valor: recepcionInfo.cartaPorte },
            { etiqueta: "Producto", valor: recepcionInfo.producto },
            { etiqueta: "Recibido", valor: recepcionInfo.netoDescontadoString },
            { etiqueta: "Vendedor", valor: recepcionInfo.vendedor }
        ]);
        return false;
    }



}

