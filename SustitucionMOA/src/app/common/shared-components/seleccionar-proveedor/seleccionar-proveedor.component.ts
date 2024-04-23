import { Component, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { SeleccionarProveedorService } from './seleccionar-proveedor.service';
import { BaseComponent } from '../../base-components/base-component';
import { FloatMsgService } from '../../services/FloatMsgService';
import { ModalService } from '../../services/ModalService';
import { NavService } from '../../services/NavService';
import { SecurityService } from '../../services/SecurityService';
import { SessionDataService } from '../../services/SessionDataService';
import { MensajeComponent } from '../../view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../view-child/spinner/spinner.component';

@Component({
    selector: 'app-seleccionar-proveedor',
    templateUrl: './seleccionar-proveedor.component.html',
    styleUrls: ['./seleccionar-proveedor.component.css'],
    providers: [SeleccionarProveedorService],
})

export class SeleccionarProveedorComponent extends BaseComponent {
    @Output() dataListed = new EventEmitter<Array<any>>();
    @ViewChild("mensajeModal")
    protected mensajeModalComponent: MensajeComponent;

    @ViewChild("spinnerModal")
    protected spinnerModalComponent: SpinnerComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: SeleccionarProveedorService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService) {

        super(navService, securytiService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.mensajeModalComponent = new MensajeComponent();
        this.spinnerModalComponent = new SpinnerComponent();
    }

    data: any;
    vendedorId: any;
    filtro: any;
    //selectProveedor: any[];
    filtroProveedor: any[];
    selected: any;
    proveedorId: any;

    ngOnInit() {
        this.getUsuario();
    }

    ngOnChanges() {
        this.getUsuario();
    }

    @Output() onLocalidadSeleccionada = new EventEmitter<any>();
    @Output() onProveedorSeleccionado = new EventEmitter<any>();
    @Output() onProveedorPreSeleccionado = new EventEmitter<any>();
    @Output() onBlur = new EventEmitter<any>();
    @Output() onFocus = new EventEmitter<any>();
    @Output() onQuery = new EventEmitter<string>();

    @Input() corredorId: number;
    @Input() tipoProveedorId: number;
    @Input() valorInicial: string;
    @Input() noEditarCliente: boolean;

    selectEvent(item) {
        this.onProveedorPreSeleccionado.emit(item);
        try {
            // console.log('SeleccionarProveedorComponent::selectEvent::item: ', item);
            this.subscription = this.service.obtenerProveedorPorCodigo(item.idVendedor).subscribe(
                (result) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        // console.log('SeleccionarProveedorComponent::selectEvent::result: ', result);
                        item = { ...item, proveedorId: result.Id, CUIT: result.CUIT }

                        this.onLocalidadSeleccionada.emit(item);
                        this.onProveedorSeleccionado.emit(item);

                    }
                },
                (error) => {
                    this.spinnerComponent.hideIt();
                    this.floatMsgService.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            return false;
        }

    }

    filterProveedor(event) {
        let filtered: any[] = [];
        let query = event.query;
        this.onQuery.emit(query)
        for (let i = 0; i < this.data.length; i++) {
            let proveedor = this.data[i];
            if (proveedor.descVendedor.toLowerCase().indexOf(query.toLowerCase()) == 0) {
                filtered.push(proveedor);
            }
        }

        this.filtroProveedor = filtered;
    }

    getUsuario() {
        this.floatMsgService.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        this.tipoProveedorId = this.tipoProveedorId ? this.tipoProveedorId : 0;

        try {
            // console.debug(' tipoProveedorId: ', this.tipoProveedorId);
            this.subscription = this.service.getVendedores("", "", this.tipoProveedorId).subscribe(
                (result) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        // console.debug(' vendedores: ', result.data.vendedores);
                        this.data = result.data.vendedores;
                        this.dataListed.emit(this.data)
                        if (this.valorInicial != "") {
                            let seleccionado = result.data.vendedores.filter(a => a.idVendedor == this.valorInicial);
                            if (seleccionado != null && seleccionado.length > 0) {
                                this.selected = seleccionado[0];
                            }
                        }
                    }
                },
                (error) => {
                    this.spinnerComponent.hideIt();
                    this.floatMsgService.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    reset() {
        this.selected = undefined;
    }

    setSelected(item) {
        this.selected = item;
        this.selectEvent(item)
    }
}