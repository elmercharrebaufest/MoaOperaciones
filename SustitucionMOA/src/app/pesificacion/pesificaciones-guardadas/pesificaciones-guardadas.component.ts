import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Pesificacion } from '../../common/models/pesificacion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { PesificacionBaseComponent } from '../pesificacion-base.component';
import { PesificacionService } from '../pesificacion.service';
import { DatePipe, formatDate } from '@angular/common';
import { FiltroFechaComponent } from '../../common/view-child/filtro-fecha/filtro-fecha.component';
import { overrideProvider } from '@angular/core/src/view';

@Component({
  selector: 'app-pesificaciones-guardadas',
  templateUrl: './pesificaciones-guardadas.component.html',
  styleUrls: ['./pesificaciones-guardadas.component.css'],
  providers: [PesificacionService]

})
export class PesificacionesGuardadasComponent extends PesificacionBaseComponent implements OnInit {


  @ViewChild("filtroFechaPesificacion")
  protected filtroFechaPesificacionComponent: FiltroFechaComponent;

  constructor(protected service: PesificacionService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    this.mensajeComponent = new MensajeComponent();
    this.spinnerComponent = new SpinnerComponent();
    this.spinnerSmallComponent = new SpinnerSmallComponent();
  }

  filtroContrato: string = "";
  tipoFiltroFecha: number = 1;
  pesificaciones: Pesificacion[] = new Array<Pesificacion>();
  filteredPesificaciones: Pesificacion[]

  ngOnInit() {
    super.ngOnInit();
    this.setMenuSeccionTab("pesificacion", "Listado");
    this.getPesificaciones()
  }


  getPesificaciones() {
    this.mensajeComponent.setMsgsEmpty();
    this.unsubscribe();
    this.subscription = this.service.getPesificacionesSap().subscribe(
      result => {
        if (result.logout == true) {
          this.sessionDataService.logout();
        } else if (result.error != undefined && result.error != "") {
          this.mensajeComponent.setErrorMsg(result.error);
        } else if (result.info != undefined) {
          this.mensajeComponent.setInfoMsg(result.info);
        } else {

          this.pesificaciones = result.data;
          this.filteredPesificaciones = this.pesificaciones;
          this.actualizarFiltroFecha();
        }
      },
      error => {
        this.mensajeComponent.setErrorMsg(error.message);
      }
    );

    return false;
  }

    actualizarFiltroFecha() {
    var fechaDesde = this.filtroFechaComponent.fecha_inicio;
    var fechaHasta = this.filtroFechaComponent.fecha_fin + " 23:59:59";

    console.log("Aplicado el filtro");
    console.log("filtroFechaInicio:", fechaDesde)
    console.log("filtroFechaInicio:", new Date(fechaDesde))
    console.log("filtroFechaFin:", fechaHasta)
    console.log("filtroFechaPesificacionInicio:", this.filtroFechaPesificacionComponent.fecha_inicio)
    console.log("filtroFechaPesificacionFin:", this.filtroFechaPesificacionComponent.fecha_fin)

    console.log(this.pesificaciones);

    this.pesificaciones.forEach(x => {
      console.log("x.FechaCarga:", x.FechaCarga)
      console.log("x.FechaCargaDate:", x.FechaCargaDate)
      console.log("x.FechaCargaDateDate:", new Date(x.FechaCargaDate))
      console.log("x.FechaCargaDateDate:", new Date(Date.parse(x.FechaCargaDate)))
    }
    )

    this.filteredPesificaciones =
      this.pesificaciones
        .filter(x =>
          new Date(Date.parse(this.tipoFiltroFecha == 1 ? x.FechaCargaDate : x.FechaPesificacionDate)) >= new Date(fechaDesde) &&
          new Date(Date.parse(this.tipoFiltroFecha == 1 ? x.FechaCargaDate : x.FechaPesificacionDate)) <= new Date(fechaHasta)
          // new Date(Date.parse(x.FechaPesificacionDate)) >= new Date(this.filtroFechaPesificacionComponent.fecha_inicio) &&
          // new Date(Date.parse(x.FechaPesificacionDate)) <= new Date(this.filtroFechaPesificacionComponent.fecha_fin)
        )

    console.log("filteredPesificaciones:", this.filteredPesificaciones)

    if (this.filteredPesificaciones.length == 0) {
      this.mensajeComponent.setInfoMsg("No se encontraron pesificaciones")
    }
    else {
      this.mensajeComponent.setMsgsEmpty();
    }
  }

}
