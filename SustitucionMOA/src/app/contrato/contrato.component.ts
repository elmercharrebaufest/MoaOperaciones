import { Component } from '@angular/core';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { Seccion } from './../common/models/seccion';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { NavService } from './../common/services/NavService';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { ContratoService } from './contrato.service';



@Component({
    selector: 'app-contrato',
    template: ``,
    providers: [ContratoService]
})
export class ContratoBaseComponent extends ListBaseComponent {

    constructor(protected service: ContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroProducto: any = null;
    filtroVendedor: any = null;
    filtroTipoContrato: any = null;
    filtroContrato: string = "";
    productoSelected: string = "";
    vendedorSelected: string = "";
    tipoContratoSelected: string = "";

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CONTRATOS"); }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList(
            [
                new Seccion('/contrato/vigente', 'contrato', 'Vigentes'),
                new Seccion('/contrato/fijacion', 'contrato', 'Fijaciones'),
                new Seccion('/contrato/ampliacion', 'contrato', 'Ampliaciones'),
                new Seccion('/contrato/anulacion', 'contrato', 'Anulaciones')
            ]
        );
        this.getData();
    }

    setFiltroProducto(producto: string) {
        this.productoSelected = producto;
    }


    setFiltroVendedor(vendedor: string) {
        this.vendedorSelected = vendedor;
    }

    setFiltroTipoContrato(tipoContrato: string) {
        this.tipoContratoSelected = tipoContrato;
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
        this.filtroTipoContrato = null;
        this.filtroContrato = "";
        this.productoSelected = "";
        this.vendedorSelected = "";
    }

    protected cargarFiltrosVariables(result: any) {
        if (result.filtroProducto != undefined) this.filtroProducto = result.filtroProducto.options;
        if (result.filtroVendedor != undefined) this.filtroVendedor = result.filtroVendedor.options;
        if (result.filtroTipoContrato != undefined) this.filtroTipoContrato = result.filtroTipoContrato.options;
    }
}