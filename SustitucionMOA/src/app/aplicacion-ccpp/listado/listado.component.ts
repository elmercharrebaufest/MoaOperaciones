import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { AplicacionCcppBaseComponent } from '../aplicacion-ccpp.base.component';
import { AplicacionCCPP, EstadoAplicacionCCPP, SeccionAplicacionCCPP } from '../aplicacion-ccpp.model';
import { AplicacionCcppService } from '../aplicacion-ccpp.service';
import { DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';

import * as XLSX from 'xlsx';

@Component({
  selector: 'app-listado',
  templateUrl: './listado.component.html',
  styleUrls: ['./listado.component.css']
})
export class ListadoComponent extends AplicacionCcppBaseComponent implements OnInit, AfterViewInit {
  @ViewChild("FiltroEstado") filtroEstadoComponent: DropdownComponent;
  aplicaciones?: AplicacionCCPP[];
  contratoFiltro = new FormControl();
  ccppFiltro = new FormControl();
  estadoFiltro = new FormControl();
  disabled = false;
  show = false;

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
  getListadoFechas() {
    this.show = false;
    this.getListado();
    this.setMenuSeccionTab(SeccionAplicacionCCPP, "Estado de cargas");
  }
  getListado() {
    this.aplicaciones = [
      {
        Id: 1,
        FechaAlta: new Date(),
        FechaActualizacion: new Date(),
        Contrato: "00123024EF",
        CartaPorte: "090009090",
        Kilogramos: 300000,
        Estado: EstadoAplicacionCCPP.Pendiente,
        ColorEstado: 'orange',
        LabelEstado: 'Pendiente',
        MailUsuario: 'mail@testin.com',
        RazonSocial: 'razon sociale',
      },
      {
        Id: 1,
        FechaAlta: new Date(),
        FechaActualizacion: new Date(),
        Contrato: "00123024EF",
        CartaPorte: "090009090",
        Kilogramos: 300000,
        Estado: EstadoAplicacionCCPP.Error,
        Error: "todo mal loco",
        ColorEstado: 'red',
        LabelEstado: 'Error',
        MailUsuario: 'mail@testin.com',
        RazonSocial: 'razon sociale',
      },
      {
        Id: 1,
        FechaAlta: new Date(),
        FechaActualizacion: new Date(),
        Contrato: "00123024EF",
        CartaPorte: "090009090",
        Kilogramos: 300000,
        Estado: EstadoAplicacionCCPP.Aplicado,
        ColorEstado: 'green',
        LabelEstado: 'Aplicado',
        MailUsuario: 'mail@testin.com',
        RazonSocial: 'razon sociale',
      },
      {
        Id: 1,
        FechaAlta: new Date(),
        FechaActualizacion: new Date(),
        Contrato: "00123024EF",
        CartaPorte: "090009090",
        Kilogramos: 300000,
        Estado: EstadoAplicacionCCPP.Pendiente,
        ColorEstado: 'orange',
        LabelEstado: 'Pendiente',
        MailUsuario: 'mail@testin.com',
        RazonSocial: 'razon sociale',
      },
    ]
    this.show = true
  }

  ngAfterViewInit(): void {
    this.filtroEstadoComponent.setSelectItem("");
  }

  exportExcelAplicacionesCCPP() {
    this.mensajeComponent.setMsgsEmpty();
    let informacionExportar: any;

    informacionExportar = this.aplicaciones.map(info => {
      return {
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
}
