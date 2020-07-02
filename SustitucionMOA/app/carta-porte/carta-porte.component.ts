import { Component, OnInit, ViewChild } from '@angular/core';
import { CartaPorteService } from './carta-porte2.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { Seccion } from './../common/models/seccion';
import { ModalService } from './../common/services/ModalService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Component({
    selector: 'my-app',
    templateUrl: `./app/carta-porte/carta-porte.component.html?v=${new Date().getTime()}`,
    providers: [CartaPorteService]
})
export class CartaPorteBaseComponent extends ListBaseComponent {

    constructor(protected service: CartaPorteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroCCPP: string = "";
    filtroProducto: any = null;
    productoSelected: string = "";
    filtroVendedor: any = null;
    vendedorSelected: string = "";

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        var secciones = [new Seccion('/carta-porte/descarga', 'carta-porte', 'Descargas'), new Seccion('/carta-porte/aplicacion', 'carta-porte', 'Aplicaciones')];
        if (this.isAuthorized("CREAR FORMULARIO CCPP"))
            secciones.push(new Seccion('/carta-porte/formulario', 'carta-porte', 'Formulario'));
        this.navService.setSeccionList(secciones);
        this.getData();
    }

    setFiltroProducto(producto: string) {
        this.productoSelected = producto;
    }


    setFiltroVendedor(vendedor: string) {
        this.vendedorSelected = vendedor;
    }

    isVisible(): boolean {
        if (this.data && this.data.cartasPorte.length != 0)
            return true;
        else
            return false;
    }

    protected vaciarFiltros() {
        this.filtroProducto = null;
        this.filtroVendedor = null;
        this.filtroCCPP = "";
        this.productoSelected = "";
        this.vendedorSelected = "";
    }

    protected cargarFiltrosVariables(result: any) {
        if (result.filtroProducto != undefined) this.filtroProducto = result.filtroProducto.options;
        if (result.filtroVendedor != undefined) this.filtroVendedor = result.filtroVendedor.options;
    }       
}