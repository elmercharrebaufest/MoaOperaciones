import { Component, ViewChild, OnInit } from '@angular/core';
import { UsuarioService } from './../usuario.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
import { BuscadorService } from '../../common/shared-components/buscador/buscador.service';
import { Proveedor } from '../../common/models/proveedor';
import { TipoPerfil } from '../../common/enums/TipoPerfil';
import { ComunicacionesService } from './../../comunicaciones/comunicaciones.service';
import { DatePipe } from '@angular/common';
import { UpdateComunicacionService } from './../../common/services/UpdateComunicacionService';

@Component({
    selector: 'app-usuario-cambio-vendedor',
    templateUrl: `usuario.cambio-vendedor.component.html`,
    providers: [UsuarioService,ComunicacionesService]
})
export class UsuarioCambioVendedorComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: UsuarioService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        protected readonly buscadorService: BuscadorService,
        protected comunicacionesService: ComunicacionesService,
        private datePipe: DatePipe,
        private updateComunicacionService: UpdateComunicacionService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: Proveedor[];
    orderedByColumn: string = "id";
    orderDirection: number = 1;
    itemsPerPage = 20;
    filtroUsuarioVendedor: string = "";
    startDate: string = "";
    endDate : string = "";

    setTabs() {
        this.setMenuSeccionTab('usuario', 'Cambio Vendedor');
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("SELECCIONAR VENDEDOR");
        this.navService.setSeccionList([]);
        this.getVendedores();
    }

    getVendedores() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.getVendedores().subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data.usuarios;
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    isVisible() {
        return this.data && this.data.length != 0;
    }

    orderColumnBy(column: string) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        } else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    }

    seleccionarVendedor(vendedor: Proveedor) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.buscadorService.limpiarBuscador();
        try {
            this.unsubscribe();
            this.subscription = this.service.seleccionarVendedor(vendedor.Id).subscribe(
                (result) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        sessionStorage.setItem("proveedor", result.CodigoVendedor);
                        this.sessionDataService.setProveedor(result.CodigoVendedor);
                        sessionStorage.setItem("nombre", result.Descripcion);
                        this.sessionDataService.setNombre(result.Descripcion);
                        sessionStorage.setItem("noticias", JSON.stringify(result.Noticias));
                        this.sessionDataService.setNoticias(result.Noticias);
                        sessionStorage.setItem("esCodigoCorredor", result.EsCodigoCorredor + '');
                        this.sessionDataService.setEsCodigoCorredor(result.EsCodigoCorredor);
                        sessionStorage.setItem("proveedorId", result.ProveedorId.toString());
                        this.sessionDataService.setProveedorId(result.ProveedorId.toString());
                        if (!this.isCorredor() && result.TipoUsuario !== TipoPerfil.Corredor) {
                            const nuevoTipoUsuario = result.TipoUsuario === TipoPerfil.Cliente ?
                                TipoPerfil.Cliente : TipoPerfil.Proveedor
                            sessionStorage.setItem("tipoUsuario", nuevoTipoUsuario);
                            this.sessionDataService.setTipoUsuario(nuevoTipoUsuario);
                            //llamada a busqueda de notificaciones del proveedor seleccionado
                            this.updateComunicacionService.updateCommunications(result.vendedor, this.startDate, this.endDate);
                        }
                    }
                    //Demora manual para visualizar la tardanza de la busqueda de notificaciones
                    setTimeout(() => {
                        this.spinnerComponent.hideIt();
                    }, 4000);
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    dateConvert() {
        var today = new Date();

        this.endDate = today.toLocaleDateString('en-US', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        })

        today.setMonth(today.getMonth() - 2);

        this.startDate = today.toLocaleDateString('en-US', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        })

        this.startDate = this.datePipe.transform(this.startDate, 'yyyy-MM-dd');
        this.endDate = this.datePipe.transform(this.endDate, 'yyyy-MM-dd');

    }
}