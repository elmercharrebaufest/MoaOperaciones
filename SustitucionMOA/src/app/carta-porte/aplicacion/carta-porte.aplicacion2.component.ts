import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { CartaPorteBaseComponent } from './../carta-porte.component';
import { CartaPorteService, CartaPorteAplicacionService } from './../carta-porte2.service';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { ActivatedRoute } from '@angular/router';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';

@Component({
    selector: 'app-carta-porte-aplicacion',
    templateUrl: `carta-porte.aplicacion.component.html`,
    providers: [{ provide: CartaPorteService, useClass: CartaPorteAplicacionService }]
})
export class CartaPorteAplicacionComponent extends CartaPorteBaseComponent {

    tituloArchivo = "ReporteAplicaciones.xls";

    constructor(protected service: CartaPorteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService, route); 
    }

    filtroFechaKey: string = 'GCPAplic_Periodo';
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimaSemana;

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CARTAS PORTE"); }

    setTabs() {
        this.setMenuSeccionTab("carta-porte", "Aplicaciones");
    }

    showModalTableResponsive(recepcionInfo: any) {
        this.modalService.openModalTableResponsive("Carta de Porte", [
            { etiqueta: "Fecha", valor: recepcionInfo.fechaDescarga },
            { etiqueta: "CCPP Nº", valor: recepcionInfo.cartaPorte },
            { etiqueta: "Producto", valor: recepcionInfo.producto },
            { etiqueta: "Recibido", valor: recepcionInfo.netoDescontadoString },
            { etiqueta: "Aplicado", valor: recepcionInfo.aLiquidarString },
            { etiqueta: "Contrato Molinos", valor: recepcionInfo.contrnum },
            { etiqueta: "Contrato Proveedor", valor: recepcionInfo.contrvend },
            { etiqueta: "Vendedor", valor: recepcionInfo.vendedor }
        ]);
        return false;
    }


}