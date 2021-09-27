import { Component, OnInit, ViewChild } from '@angular/core';
import { LogPesificacionViewModel } from '../ViewModels/logPesificacionesViewModel';
import { FiltroPesificacionViewModel } from '../ViewModels/filtroPesificacionesViewModel';
import { LogPesificacionService } from '../log-pesificacion.service';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { SessionDataService } from '../../common/services/SessionDataService';
import { Router, ActivatedRoute } from '@angular/router';
import { NavService } from '../../common/services/NavService';
declare var $: any;

@Component({
  selector: 'app-listado-log-pesificacion',
  templateUrl: './listado-log-pesificacion.component.html'
})

export class ListadoLogPesificacionComponent implements OnInit {

  public listadoPesificaciones: Array<LogPesificacionViewModel> = [];
  public filtros: FiltroPesificacionViewModel = new FiltroPesificacionViewModel();
  public orderedByColumn: string = "contrato";
  public orderDirection: number = 1;
  public itemsPerPage = "10";
  public fechaFiltro: any;

  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  @ViewChild(SpinnerSmallComponent)
  protected spinnerSmallComponent: SpinnerSmallComponent;

  constructor(private logEspecificacionService: LogPesificacionService,
    protected sessionDataService: SessionDataService,
    private route: ActivatedRoute,
    private router: Router,
    protected navService: NavService) {
    this.mensajeComponent = new MensajeComponent();
    this.spinnerComponent = new SpinnerComponent();
    this.spinnerSmallComponent = new SpinnerSmallComponent();
  }

  ngOnInit() {
    this.navService.setSeccionActive("Pesificaciones Manuales");
    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();

    $('.form_datetime_Inicio').datetimepicker({
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
      maxView: 4
    });

    this.inicliazarVista();
  }

  public inicliazarVista(): void {
    this.logEspecificacionService.getLogPesificaciones().subscribe(
      (result:any) => {
        this.spinnerComponent.hideIt();
        if (result.logout == true) {
          this.sessionDataService.logout();
        } else if (result.error != undefined && result.error != "") {
          this.mensajeComponent.setErrorMsg(result.error);
        } else if (result.info != undefined) {
          this.mensajeComponent.setInfoMsg(result.info);
        } else {
          this.listadoPesificaciones = result.data.map((logPesificacion) => {
            return new LogPesificacionViewModel(logPesificacion);
          });
        }
      },
      error => {
        this.spinnerComponent.hideIt();
        this.mensajeComponent.setErrorMsg(error.message);
        return false;
      }
    );
  }

  public filtrarResultatos(): void {
    this.spinnerComponent.showIt();
    this.listadoPesificaciones = [];
    this.filtros.fecha = $("#dtp_input_inicio").val();
    this.logEspecificacionService.getFiltrarPesificaciones(this.filtros).subscribe(
      (result:any) => {
        this.spinnerComponent.hideIt();
        if (result.logout == true) {
          this.sessionDataService.logout();
        } else if (result.error != undefined && result.error != "") {
          this.mensajeComponent.setErrorMsg(result.error);
        } else if (result.info != undefined) {
          this.mensajeComponent.setInfoMsg(result.info);
        } else {
          this.listadoPesificaciones = result.data.map((logPesificacion) => {
            return new LogPesificacionViewModel(logPesificacion);
          });
        }
      },
      error => {
        this.spinnerComponent.hideIt();
        this.mensajeComponent.setErrorMsg(error.message);
        return false;
      }
    );
  }

  public isVisiblePaginacion(): boolean {
    return this.listadoPesificaciones != null && this.listadoPesificaciones.length > 0;
  }

  public orderColumnBy(column: string): void {
    if (column == this.orderedByColumn) {
      this.orderDirection = -this.orderDirection;
    } else {
      this.orderDirection = 1;
      this.orderedByColumn = column;
    }
  }

  public round(num: number): number {
    return Math.round(num);
  };

}
