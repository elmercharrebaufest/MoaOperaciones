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
import { finalize } from 'rxjs/operators';
import { ArchivoDescarga } from '../common/models/archivoDescarga';
import { ConsultaTicketPesadaSubproductos } from '../common/models/ticket-pesada/consulta-ticket-pesada';
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
  buscandoDatos: boolean = false;
  hayDatos: boolean = false;
  data: Array<ArchivoDescarga> = [];
  archivoZip: ArchivoDescarga = null;
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

    // Si es un objeto Date, devolverlo directamente
    if (fecha instanceof Date && !isNaN(fecha.getTime())) {
      return fecha;
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

      // Intenta parsear formato dd/MM/yyyy o dd/MM/yyyy HH:mm:ss
      // Soporta separadores /, -, y espacios
      const datePattern = /(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})(?:\s+(\d{1,2}):(\d{1,2})(?::(\d{1,2}))?)?/;
      const dateMatch = datePattern.exec(fecha.trim());
      
      if (dateMatch) {
        const day = parseInt(dateMatch[1], 10);
        const month = parseInt(dateMatch[2], 10) - 1; // 0-indexed
        const year = parseInt(dateMatch[3], 10);
        const hours = dateMatch[4] ? parseInt(dateMatch[4], 10) : 0;
        const minutes = dateMatch[5] ? parseInt(dateMatch[5], 10) : 0;
        const seconds = dateMatch[6] ? parseInt(dateMatch[6], 10) : 0;
        
        // Validar valores para evitar fechas inválidas
        if (day >= 1 && day <= 31 && month >= 0 && month <= 11 && year >= 1900) {
          const rebuilt = new Date(year, month, day, hours, minutes, seconds);
          if (!isNaN(rebuilt.getTime())) {
            return rebuilt;
          }
        }
      }

      // Último intento: crear Date directo (ISO u otros formatos)
      const direct = new Date(fecha);
      if (!isNaN(direct.getTime())) {
        return direct;
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

    fechaEgreso.setHours(23, 59, 0, 0);

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

  descargarPdf(ticket: any) {
    const ticketPesada: ConsultaTicketPesadaSubproductos = new ConsultaTicketPesadaSubproductos();
    const fechaInicio = this.parseFecha(ticket.FechaHoraIngreso) || new Date();
    ticketPesada.FechaDesde = fechaInicio;
    ticketPesada.FechaHasta = fechaInicio;
    ticketPesada.PatenteCamion = ticket.Patente;

    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    this.unsubscribe();
    this.buscandoDatos = true;

    this.subscription = this.ticketPesadaService
      .ObtenerTicketPesadaSubproductos(ticketPesada)
      .pipe(finalize(() => (this.buscandoDatos = false)))
      .subscribe(
        (result) => {
          this.spinnerComponent.hideIt();
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (
            result.error != undefined &&
            result.error != ""
          ) {
            this.mensajeComponent.setErrorMsg(result.error);
          } else if (result.info != undefined) {
            this.mensajeComponent.setInfoMsg(result.info);
          } else {
            result.data.forEach((file) => {
              try {
                if (!file.Datos) {
                  console.error('No hay datos para el archivo:', file.Nombre);
                  return;
                }

                let byteArray: Uint8Array;

                if (Array.isArray(file.Datos) || file.Datos instanceof Uint8Array || file.Datos instanceof ArrayBuffer) {
                  byteArray = new Uint8Array(file.Datos);
                } else if (typeof file.Datos === 'string') {
                  const base64Data = file.Datos.replace(/\s/g, '');

                  if (!/^[A-Za-z0-9+/]*={0,2}$/.test(base64Data)) {
                    console.error('Formato base64 inválido para el archivo:', file.Nombre);
                    this.mensajeComponent.setErrorMsg(`Error al decodificar el archivo ${file.Nombre}. Formato inválido.`);
                    return;
                  }

                  const byteCharacters = atob(base64Data);
                  const byteNumbers = new Array(byteCharacters.length);
                  for (let i = 0; i < byteCharacters.length; i++) {
                    byteNumbers[i] = byteCharacters.charCodeAt(i);
                  }
                  byteArray = new Uint8Array(byteNumbers);
                } else {
                  console.error('Tipo de datos no soportado para el archivo:', file.Nombre, typeof file.Datos);
                  this.mensajeComponent.setErrorMsg(`Tipo de datos no soportado para el archivo ${file.Nombre}.`);
                  return;
                }

                const blob = new Blob([new Uint8Array(byteArray)], { type: 'application/octet-stream' });
                const link = document.createElement('a');
                const url = URL.createObjectURL(blob);
                link.setAttribute('href', url);
                link.setAttribute('download', file.Nombre);
                link.style.visibility = 'hidden';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                URL.revokeObjectURL(url);
              } catch (error) {
                console.error('Error al descargar el archivo:', file.Nombre, error);
                this.mensajeComponent.setErrorMsg(`Error al descargar el archivo ${file.Nombre}: ${error.message}`);
              }
            });
          }
        },
        (error) => {
          this.spinnerComponent.hideIt();
          this.mensajeComponent.setErrorMsg(error.message);
          this.buscandoDatos = false;
        }
      );
  }

}
