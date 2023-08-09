import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { AplicacionCcppBaseComponent } from '../aplicacion-ccpp.base.component';
import { AplicacionCCPP, SeccionAplicacionCCPP, AplicacionCCPPFiltro } from '../aplicacion-ccpp.model';
import { AplicacionCcppService, ListadoRequest } from '../aplicacion-ccpp.service';
import { DropdownComponent, DropdownOption } from '../../common/view-child/dropdown/dropdown.component';

import * as XLSX from 'xlsx';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
  selector: 'app-listado',
  templateUrl: './listado.component.html',
  styleUrls: ['./listado.component.css']
})
export class ListadoComponent extends AplicacionCcppBaseComponent implements OnInit, OnDestroy {
  @BlockUI() blockUI: NgBlockUI;
  @ViewChild("FiltroEstado") filtroEstadoComponent: DropdownComponent;
  aplicaciones?: AplicacionCCPP[];
  opcionesClientes?: DropdownOption[];
  opcionesEstadoAplicacionCCPP?: DropdownOption[];

  contratoFiltro = new FormControl();
  ccppFiltro = new FormControl();
  estadoFiltro = new FormControl();
  clienteFiltro = new FormControl();
  esAdmin = this.isAuthorized('ADMIN APLICACIONES CCPP');
  disabled = false;
  show = false;
  develop = true;
  constructor(
    protected service: AplicacionCcppService,
    protected navService: NavService,
    protected sessionDataService: SessionDataService,
    protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService,
    protected modalService: ModalService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  ngOnInit() {
    this.crearSecciones();
    this.setMenuSeccionTab(SeccionAplicacionCCPP, 'Estado de cargas');
    this.getListado()
  }
  get isVisible(): boolean {
    return this.aplicaciones && !!this.aplicaciones.length && this.show
  }
  get verUsuario(): boolean {
    return true;
  }
  get verRazonSocial(): boolean {
    return true;
  }
  get filtraClientes(): boolean {
    return this.esAdmin;
  }
  getListadoFechas() {
    this.getListado();
    this.setMenuSeccionTab(SeccionAplicacionCCPP, "Estado de cargas");
  }
  getListado() {
    this.mensajeComponent.setMsgsEmpty();
    this.limpiarListado()
    this.blockUI.start('');
    this.disabled = true;
    this.unsubscribe();
    this.subscription =
      this.service.getListado(this.getRequest()).subscribe(res => {
        this.blockUI.stop();
        if (res.logout)
          this.sessionDataService.logout()
        else if (res.info)
          this.mensajeComponent.setInfoMsg(res.info)
        else if (res.error)
          this.mensajeComponent.setErrorMsg(res.error)
        else {
          this.show = true
          this.aplicaciones = res.data
          this.setOpciones(res.filtros)
        }
      });
  }
  limpiarListado() {
    this.show = false
    this.aplicaciones = []
  }

  exportExcelAplicacionesCCPP() {
    this.mensajeComponent.setMsgsEmpty();
    let informacionExportar: any;

    informacionExportar = this.aplicaciones.map(info => {
      return {
        "Fecha": info.FechaAlta || "-",
        "Usuario": info.MailUsuario || "-",
        "Razon Social": info.RazonSocial || "-",
        "Contrato": info.Contrato,
        "Carta Porte": info.CartaPorte || "-",
        "KGS": info.Kilogramos || "-",
        "Estado": info.LabelEstado || "-",
        "Observaciones": info.Error,
      }
    });

    if (informacionExportar.length == 0) {
      this.mensajeComponent.setInfoMsg("No existen datos para exportar.");
      return
    }

    this.DownloadJsonData(informacionExportar, "Aplicación CCPP");
  }

  DownloadJsonData(JSONData: any, FileTitle: string) {
    //crea la estructura inicial del archivo
    let worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(JSONData);
    let workbook: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, FileTitle);
    //escribe el file para ser descargado
    XLSX.writeFile(workbook, FileTitle + '.xlsx');
  }

  getRequest(): ListadoRequest {
    return {
      fechaInicio: this.filtroFechaComponent.fecha_inicio,
      fechaFin: this.filtroFechaComponent.fecha_fin
    }
  }
  setOpciones({ FiltroEstados, FiltroClientes }: AplicacionCCPPFiltro) {
    this.opcionesEstadoAplicacionCCPP = FiltroEstados;
    this.opcionesClientes = FiltroClientes;
  }
  eliminarAplicacion(aplicacion: AplicacionCCPP) { }
}
