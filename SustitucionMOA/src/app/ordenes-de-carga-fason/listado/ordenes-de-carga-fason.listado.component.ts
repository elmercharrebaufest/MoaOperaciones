import { Component, OnInit, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { NgBlockUI, BlockUI } from 'ng-block-ui';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { OrdenesDeCargaFasonService } from '../ordenes-de-carga-fason.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { Material } from '../../common/models/material';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-listado',
    templateUrl: './ordenes-de-carga-fason.listado.component.html',
    styleUrls: ['./ordenes-de-carga-fason.listado.component.css']
})
export class OrdenesDeCargaFasonListadoComponent extends ListBaseComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;

  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  productoSelected: string = "Todos";
  estadosSelected: string[] = [
    "Generada",
    "Pendiente",
    //"Vencida",
    //"Entregada"
  ];
  descripcionEstadoOrdenCarga: any[];
  listaProductos: Material[];
  datosAux: any[];
  primerListado: any[];
  filtroCliente: any = null;
  filtroPatente: any = null;
  entregada: string = "Entregada";

  esCliente: boolean = sessionStorage.getItem("tipoUsuario") === "CLI";

  //esTercero: boolean = this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS');

  constructor(protected service: OrdenesDeCargaFasonService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private confirmationService: ConfirmationService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  ngOnInit = () => {
    console.debug('OrdenesDeCargaFasonListado - ngOnInit()');
    this.descripcionEstadoOrdenCarga = [
      { label: "Generada", value: "Generada" },
      { label: "Pendiente", value: "Pendiente" },
      //{ label: "Vencida", value: "Vencida" },
      { label: "Entregada", value: "Entregada" }
    ];

    this.setTabs();
    this.checkPermisos();
    this.navService.setSeccionList([]);
    this.getListado();
    this.getProductos();
  }

  getListado = (resultMessage?: string) => {
    this.spinnerComponent.showIt();
    if (resultMessage != undefined) {
        this.mensajeComponent.setSuccessMsg(resultMessage);
    } else {
        this.mensajeComponent.setMsgsEmpty();
    }
    this.data = null;
    try {
      this.unsubscribe();
      this.subscription = this.service.listado(this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin)
        .subscribe(result => {
            // this.spinnerComponent.hideIt();
            // if (result.logout == true) {
            //     this.sessionDataService.logout();
            // } else if (result.error != undefined && result.error != "") {
            //     this.mensajeComponent.setErrorMsg(result.error);
            // } else if (result.info != undefined) {
            //     this.mensajeComponent.setInfoMsg(result.info);
            // } else {
                this.data = result.Response;
                this.datosAux = result.Response;
                this.filtrarListado();
            // }
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
    this.spinnerComponent.hideIt();
    return false; //<-- Prevent Refresh
  }

  filtrarListado = () => {
    this.primerListado = this.datosAux.filter(x => x.Estado != this.entregada);
    //if (!this.esTercero) {
      if (this.estadosSelected.length < 1 || this.estadosSelected == null) {
        this.data = this.datosAux;
      } else {
        // console.debug(' estadosSelected: ', this.estadosSelected);
        if (this.estadosSelected) {
            //this.data = this.datosAux.filter(x => this.estadosSelected.indexOf(x.DescripcionEstado) >= 0);
            this.data = this.datosAux.filter(x => this.estadosSelected.some(y => y == x.Estado));
        }
      }
    //} else {
    //  this.data = this.datosAux;
    //}

  }

  setFiltroProducto = (producto: string) => {
    this.productoSelected = producto;
  }

  getProductos = () => {
    // console.debug('getMateriales()');
    this.subscription = this.service.getMateriales().subscribe(
        (result) => {
            this.listaProductos = result.data;
        },
        (error) => {
            this.mensajeComponent.setErrorMsg(error.message);
        }
    );
  }
}
