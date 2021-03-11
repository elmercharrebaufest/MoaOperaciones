import { Component, OnInit, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { SeleccionarProveedorService } from './seleccionar-proveedor.service';
import { FormControl } from '@angular/forms';
import { BaseComponent } from '../../base-components/base-component';
import { InformeComercial } from '../../models/informeComercial';
import { Material } from '../../models/material';
import { NuevoAcopio } from '../../models/nuevoAcopio';
import { NuevoProduccion } from '../../models/nuevoProduccion';
import { FloatMsgService } from '../../services/FloatMsgService';
import { ModalService } from '../../services/ModalService';
import { NavService } from '../../services/NavService';
import { SecurityService } from '../../services/SecurityService';
import { SessionDataService } from '../../services/SessionDataService';
import { SpinnerSmallComponent } from '../../view-child/spinner-small/spinner-small.component';
import { MensajeComponent } from '../../view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../view-child/spinner/spinner.component';

@Component({
  selector: 'app-seleccionar-proveedor',
  templateUrl: './seleccionar-proveedor.component.html',
  styleUrls: ['./seleccionar-proveedor.component.css'],
  providers: [SeleccionarProveedorService],
})

export class SeleccionarProveedorComponent extends BaseComponent implements OnInit {

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
  selectProveedor: any[];
  filtroProveedor: any[];
  selected: any;

  ngOnInit() {
    this.getUsuario();
  }
  
  @Output() onLocalidadSeleccionada = new EventEmitter<any>();

  @Input() corredorId:number;

  selectEvent(item) {
    this.onLocalidadSeleccionada.emit(item);
  }

  filterProveedor(event) {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.data.length; i++) {
      let proveedor = this.data[i];
      if (proveedor.descVendedor.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(proveedor);
      }
    }

    this.filtroProveedor = filtered;
  }

  getUsuario() {
    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    this.unsubscribe();
    try {
        this.subscription = this.service.getVendedores("", "").subscribe(
            (result) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.data = result.data.vendedores;
                    this.data.sort((a, b) => (a.descVendedor > b.descVendedor) ? 1 : -1)
                    this.selectProveedor = [];
                    this.data.forEach(x => this.selectProveedor.push({ label: x.descVendedor, value: x.idVendedor}));
                }
            },
            (error) => {
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
}