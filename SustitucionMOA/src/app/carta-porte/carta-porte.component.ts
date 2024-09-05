import { Component, OnInit, ViewChild, Directive } from '@angular/core';
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
import { ActivatedRoute } from '@angular/router';



@Directive({
    selector: 'app-carta-porte',
    providers: [CartaPorteService]
})
export class CartaPorteBaseComponent extends ListBaseComponent {

    constructor(protected service: CartaPorteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService, );
    }

    filtroCCPP: string = "";
    filtroProducto: any = null;
    filtroContrato: string = "";
    productoSelected: string = "";
    filtroVendedor: any = null;
    vendedorSelected: string = "";
    msgBusquedaLimitada: string = "";
    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        var secciones = [new Seccion('/carta-porte/descarga', 'carta-porte', 'Descargas'), new Seccion('/carta-porte/aplicacion', 'carta-porte', 'Aplicaciones')];
        if (this.isAuthorized("CREAR FORMULARIO CCPP")) {
            //secciones.push(new Seccion('/carta-porte/formulario', 'carta-porte', 'Formulario'));
        }
        this.navService.setSeccionList(secciones);

        if (this.route.snapshot.paramMap.get('contrato')) {
            sessionStorage.setItem("periodo", "4");
            this.filtroFechaComponent.periodo = "4";
            var fechaActualMenos5años = this.restarAñosDateActual(new Date())
            this.filtroFechaComponent.fecha_inicio = this.fechasParaFiltros(fechaActualMenos5años);
            this.filtroFechaComponent.fecha_fin = this.fechasParaFiltros(new Date());
            this.filtroContrato = this.route.snapshot.paramMap.get('contrato')
            this.msgBusquedaLimitada = "Búsqueda limitada a los últimos 6 meses, modificar el rango fechas para buscar anteriores.";
        }

        this.getData();
    }

    fechasParaFiltros(date: Date) {
        var mm = date.getMonth() + 1; // getMonth() is zero-based
        var dd = date.getDate();
      
        return [date.getFullYear(), "-",
                (mm>9 ? '' : '0') + mm, "-",
                (dd>9 ? '' : '0') + dd
               ].join('');
    };

    restarAñosDateActual(d: Date){
        new Date();
        var year = d.getFullYear();
        var month = d.getMonth();
        var day = d.getDate();
        var c = new Date(year, month - 6, day);

        return c
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