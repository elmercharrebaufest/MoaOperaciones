import { Component, OnInit, ViewChild } from '@angular/core';
import { ContratoService } from './contrato.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { Seccion } from './../common/models/Seccion';



@Component({
    selector: 'app-contrato',
    template: ``,
    providers: [ContratoService]
})  
export class ContratoBaseComponent extends ListBaseComponent {

    constructor(protected service: ContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroProducto : any = null;
    filtroVendedor : any = null;
    filtroContrato : string = "";
    productoSelected : string = "";
    vendedorSelected : string = "";

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CONTRATOS"); }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/contrato/vigente', 'contrato', 'Vigentes'), new Seccion('/contrato/fijacion', 'contrato', 'Fijaciones'), new Seccion('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new Seccion('/contrato/anulacion', 'contrato', 'Anulaciones'), new Seccion('/crear-contrato/aprecio', 'contrato', 'Crear')]);
        this.getData();
    }

    setFiltroProducto(producto: string) {
        this.productoSelected = producto;
    }


    setFiltroVendedor(vendedor: string) {
        this.vendedorSelected = vendedor;
    }

    isVisible(): boolean {
        if (this.data && this.data.contratosInfo.length != 0)
            return true;
        else
            return false;
    }

    protected vaciarFiltros() {
        this.filtroProducto = null;
        this.filtroVendedor = null;
        this.filtroContrato = "";
        this.productoSelected = "";
        this.vendedorSelected = "";
    }

    protected cargarFiltrosVariables(result: any) {
        if (result.filtroProducto != undefined) this.filtroProducto = result.filtroProducto.options;
        if (result.filtroVendedor != undefined) this.filtroVendedor = result.filtroVendedor.options;
    }
}