import { Component, OnInit, ViewChild } from '@angular/core';
import { SecurityService } from '../common/services/SecurityService';
import { ListBaseComponent } from '../common/base-components/list-base-component';
import { NavService } from '../common/services/NavService';
import { SessionDataService } from '../common/services/SessionDataService';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ActivatedRoute, Router } from '@angular/router';
import { ModalService } from '../common/services/ModalService';
import { TicketPesadaService } from '../ticket-pesada/ticket-pesada.service';
import { Seccion } from '../common/models/seccion';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpStatusCodes } from '../common/models/httpStatusCodes';
import { MensajeComponent } from '../common/view-child/mensaje/mensaje.component';
import * as XLSX from 'xlsx';
declare var $: any;

@Component({
  selector: 'app-consulta-ticket-pesada',
  templateUrl: './consulta-ticket-pesada.component.html',
  styleUrls: ['./consulta-ticket-pesada.component.css']
})
export class ConsultaTicketPesadaComponent extends ListBaseComponent implements OnInit {
  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  fechaIngreso: string = new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate()).toLocaleDateString('en-GB');
  fechaEgreso: string = new Date().toLocaleDateString('en-GB');
  filtroPatente: string = "";
  filtroCtg: string = "";
  filtroCuitProveedor: string = "";
  filtroCuitIntermediarioFlete: string = "";
  filtroCuitTransportista: string = "";
  tickets: any[] = [];
  esAdmin: boolean = this.isAuthorized('CONSULTA TICKET PESADA ADMIN');
  cuitSesion: string = sessionStorage.getItem('cuit') || "";

  constructor(protected ticketPesadaService: TicketPesadaService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router) {
    super(ticketPesadaService, navService, sessionDataService, securityService, floatMsgService, modalService);
    this.mensajeComponent = new MensajeComponent();
  }

  checkPermisos() { this.securityService.tienePermisoRedirect("CARGAR FACT PROV"); }

  ngOnInit() {
    this.setTabs();
    this.checkPermisos();
    let secciones = [];
    secciones.push(new Seccion('/consulta-ticket-pesada', 'consulta-ticket-pesada', 'Ticket Pesada'));
    this.navService.setSeccionList(secciones);
    this.getDatosTicketPesada();
  }

  ngAfterViewInit() {
    $('.form_datetime_fechaIngreso').datetimepicker({
      format: 'dd/mm/yyyy',
      language: 'es',
      weekStart: 1,
      todayBtn: 1,
      autoclose: 1,
      todayHighlight: 1,
      startView: 2,
      forceParse: 0,
      showMeridian: 1,
      pickTime: false,
      minView: 2,
      maxView: 4,
      defaultDate: 0
    }).on('changeDate', (e) => {
      this.fechaIngreso = $('#noCursor_fechaIngreso').val() as string;
    });


    $('.form_datetime_fechaEgreso').datetimepicker({
      format: 'dd/mm/yyyy',
      language: 'es',
      weekStart: 1,
      todayBtn: 1,
      autoclose: 1,
      todayHighlight: 1,
      startView: 2,
      forceParse: 0,
      showMeridian: 1,
      pickTime: false,
      minView: 2,
      maxView: 4,
      defaultDate: 0
    }).on('changeDate', (e) => {
      this.fechaEgreso = $('#noCursor_fechaEgreso').val() as string;
    });
  }

  setTabs() {
    this.setMenuSeccionTab("consulta-ticket-pesada", "Ticket Pesada");
  }

  formatDate(date: Date): string {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  }

  parseFecha(fecha: any): Date | null {
    if (!fecha) {
      return null;
    }

    // Intenta crear Date directo (ISO u objetos Date)
    const direct = new Date(fecha);
    if (!isNaN(direct.getTime())) {
      return direct;
    }

    // Maneja formato /Date(1765393448560)/
    if (typeof fecha === 'string') {
      const match = /\/Date\((\d+)\)\//.exec(fecha);
      if (match) {
        const millis = parseInt(match[1], 10);
        const fromTicks = new Date(millis);
        if (!isNaN(fromTicks.getTime())) {
          return fromTicks;
        }
      }

      // dd/MM/yyyy o dd/MM/yyyy HH:mm:ss
      const parts = fecha.split(/[\/\s:]/);
      if (parts.length >= 3) {
        const day = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10) - 1;
        const year = parseInt(parts[2], 10);
        const hours = parts.length > 3 ? parseInt(parts[3], 10) : 0;
        const minutes = parts.length > 4 ? parseInt(parts[4], 10) : 0;
        const seconds = parts.length > 5 ? parseInt(parts[5], 10) : 0;
        const rebuilt = new Date(year, month, day, hours, minutes, seconds);
        if (!isNaN(rebuilt.getTime())) {
          return rebuilt;
        }
      }
    }

    return null;
  }

  formatearCuit(cuit: string): string {
    if (!cuit || cuit.length !== 11) {
      return cuit;
    }
    return `${cuit.substring(0, 2)}-${cuit.substring(2, 10)}-${cuit.substring(10, 11)}`;
  }

  getDatosTicketPesada() {
    this.mensajeComponent.setMsgsEmpty();
    const fechaInicio = this.parseFecha(this.fechaIngreso) || new Date();
    const fechaEgreso = this.parseFecha(this.fechaEgreso) || new Date();
      
    // Validar que el periodo no sea mayor a 1 año
     const unAnoEnMilisegundos = 365 * 24 * 60 * 60 * 1000;
     const diferenciaTiempo = fechaEgreso.getTime() - fechaInicio.getTime();
     
     if (diferenciaTiempo > unAnoEnMilisegundos) {
       this.mensajeComponent.setErrorMsg("No se puede obtener información de más de 1 año. Por favor, seleccione un rango de fechas menor.");
       return;
     }
 
    if (!this.esAdmin) {
      this.filtroCuitTransportista = this.formatearCuit(this.cuitSesion);
      this.filtroCuitIntermediarioFlete = this.formatearCuit(this.cuitSesion);
      this.filtroCuitProveedor = this.formatearCuit(this.cuitSesion);
    }

    if (!this.isCuitValido(this.filtroCuitProveedor)) {
      this.mensajeComponent.setMsgsEmpty();
      this.mensajeComponent.setErrorMsg("El cuit del proveedor tiene un formato inválido.");
      return;
    }

    if (!this.isCuitValido(this.filtroCuitIntermediarioFlete)) {
      this.mensajeComponent.setMsgsEmpty();
      this.mensajeComponent.setErrorMsg("El cuit del intermediario de flete tiene un formato inválido.");
      return;
    }

    if (!this.isCuitValido(this.filtroCuitTransportista)) {
      this.mensajeComponent.setMsgsEmpty();
      this.mensajeComponent.setErrorMsg("El cuit del transportista tiene un formato inválido.");
      return;
    }

    this.spinnerComponent.showIt();

    this.unsubscribe();
    try {
      this.subscription = this.ticketPesadaService.ListarDatosTicketPesada(fechaInicio, fechaEgreso, this.filtroCuitProveedor, this.filtroCuitTransportista, this.filtroCtg, this.filtroPatente, this.filtroCuitIntermediarioFlete, this.esAdmin).subscribe(
        (result) => {
          this.spinnerComponent.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            this.tickets = (result.data || []).map(t => ({
              ...t,
              FechaHoraIngreso: this.parseFecha(t.FechaHoraIngreso),
              FechaHoraEgreso: this.parseFecha(t.FechaHoraEgreso)
            }));
            console.log(this.tickets)
          }
        },
        (error: HttpErrorResponse) => {
          this.spinnerComponent.hideIt();
          this.mensajeComponent.setErrorMsg(HttpStatusCodes.friendlyStatusCode(error.status));
        }
      );
    } catch (e) {
      this.spinnerComponent.hideIt();
      this.mensajeComponent.setErrorMsg(e);
      return false; //<-- Prevent Refresh
    }

    return false; //<-- Prevent Refresh
  }

  isCuitValido(cuit: string): boolean {
    if (!cuit) { return true; }
    const limpio = cuit.replace(/\D/g, '');
    if (!/^\d{11}$/.test(limpio)) { return false; }
    if (cuit.includes('-') && !/^\d{2}-\d{8}-\d$/.test(cuit.trim())) { return false; }
    return true;
  }

  exportarExcel() {
    if (!this.tickets || this.tickets.length === 0) {
      this.mensajeComponent.setErrorMsg("No hay datos para exportar.");
      return;
    }

    const headers = ['CTG', 'Bruto Origen', 'Tara Origen', 'Neto Origen', 'Bruto Planta', 'Tara Planta', 'Neto Planta', 'Ingreso', 'Egreso', 'Procedencia', 'Intermediario', 'CUIT Intermediario', 'Transportista', 'CUIT Transportista', 'Patente', 'Chofer', 'Cuit Tit. CP', 'Material'];
    const rows = this.tickets.map(t => [
      t.CTG,
      t.BrutoOrigen,
      t.TaraOrigen,
      t.NetoOrigen,
      t.BrutoPlanta,
      t.TaraPlanta,
      t.NetoPlanta,
      t.FechaHoraIngreso ? new Date(t.FechaHoraIngreso).toLocaleString('es-AR') : '',
      t.FechaHoraEgreso ? new Date(t.FechaHoraEgreso).toLocaleString('es-AR') : '',
      t.Procedencia,
      t.Intermediario,
      t.IntermediarioCUIT,
      t.Transportista,
      t.TransportistaCUIT,
      t.Patente,
      t.ChoferNombreApellido,
      t.TitularCPCUIT,
      t.Material
    ]);

    // Crear libro de trabajo y hoja de cálculo
    const worksheetData = [headers, ...rows];
    const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Tickets Pesada');

    // Generar archivo XLSX y descargarlo
    XLSX.writeFile(workbook, `TicketsPesada_${new Date().toISOString().slice(0, 10)}.xlsx`);
  }

}
