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
import { Permiso } from '../../common/enums/Permisos';

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
    "Orden generada",
    "Pendiente",
    "Orden vencida",
    "Orden entregada",
    "Sin estado",
    "Edición solicitada",
    "Edición rechazada",
    "Anulación solicitada",
    "Anulada"
  ];
  descripcionEstadoOrdenCarga: any[];
  listaProductos: Material[];
  listaDestino: string[] = [];
  listaClientes: string[] = [];
  datosAux: any[];
  //primerListado: any[];
  filtroCliente: any = null;
  filtroPatente: any = null;
  //entregada: string = "Orden entregada";



  esTercero: boolean = this.isAuthorized(Permiso.FasonVerOrdenesDeCarga);
  esAdmin: boolean = this.isAuthorized(Permiso.FasonVerOrdenesDeCargaAdmin);

  constructor(protected service: OrdenesDeCargaFasonService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private confirmationService: ConfirmationService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  ngOnInit = () => {
    this.descripcionEstadoOrdenCarga = [
      { label: "Orden generada", value: "Orden generada" },
      { label: "Pendiente", value: "Pendiente" },
      { label: "Orden vencida", value: "Orden vencida" },
      { label: "Orden entregada", value: "Orden entregada" },
      { label: "Sin estado", value: "Sin estado" },
      { label: "Edición solicitada", value: "Edición solicitada" },
      { label: "Edición rechazada", value: "Edición rechazada" },
      { label: "Anulación solicitada", value: "Anulación solicitada" },
      { label: "Anulada", value: "Anulada" }
    ];

    if (!this.esAdmin) {
      this.descripcionEstadoOrdenCarga = [
        { label: "OK", value: "OK" },
        { label: "Orden vencida", value: "Orden vencida" },
        { label: "Orden entregada", value: "Orden entregada" }
      ];

      this.estadosSelected = ["OK"];
    }

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
          this.spinnerComponent.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            this.data = result.Response;
            this.datosAux = result.Response;
            this.filtrarListado();
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
    this.spinnerComponent.hideIt();
    return false; //<-- Prevent Refresh
  }

  filtrarListado() {
    // console.log("datosAux: ", this.datosAux)
    if (this.estadosSelected.length < 1 || this.estadosSelected == null) {
      this.data = this.datosAux;
    } else {
      if (this.estadosSelected) {
        this.data = this.datosAux.filter(x => this.estadosSelected.some(y => y == x.DescripcionEstado));
      }
    }
  }

  setFiltroProducto = (producto: string) => {
    this.productoSelected = producto;
  }

  getProductos = () => {
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
