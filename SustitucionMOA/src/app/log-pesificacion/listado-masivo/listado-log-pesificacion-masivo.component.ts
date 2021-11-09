import { Component, OnInit, ViewChild } from '@angular/core';
import { LogPesificacionViewModel } from '../ViewModels/logPesificacionesViewModel';
import { FiltroPesificacionesAutomaticoViewModel } from '../ViewModels/filtroPesificacionesAutomaticoViewModel';
import { LogPesificacionService } from '../log-pesificacion.service';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { SessionDataService } from '../../common/services/SessionDataService';
import { Router, ActivatedRoute } from '@angular/router';
import { NavService } from '../../common/services/NavService';
declare var $: any;

@Component({
  selector: 'app-listado-log-pesificacion-masivo',
  templateUrl: './listado-log-pesificacion-masivo.component.html'
})

export class ListadoLogPesificacionMasivoComponent implements OnInit {

  public listadoPesificaciones: Array<LogPesificacionViewModel> = [];
  public filtros: FiltroPesificacionesAutomaticoViewModel = new FiltroPesificacionesAutomaticoViewModel();
  public orderedByColumn: string = "fecha";
  public orderDirection: number = 1;
  public itemsPerPage = "10";

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
    this.navService.setSeccionActive("Pesificaciones Automaticas");
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
    this.filtros.fecha = $("#dtp_input_inicio").val();
    this.logEspecificacionService.getLogPesificacionesAutomaticas().subscribe(
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
    this.logEspecificacionService.getFiltrarPesificacionesAutomaticas(this.filtros).subscribe(
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

  descargarArchivo(pesificacion: LogPesificacionViewModel) {
    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    this.logEspecificacionService
      .descargarArchivoSubido(pesificacion.idArchivo)
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
            var byteArray = new Uint8Array(result.FileContents);
            var blob = new Blob([byteArray], {
              type: "application/octet-stream",
            });

            if (window.navigator.msSaveOrOpenBlob) {
              // IE11
              window.navigator.msSaveOrOpenBlob(
                blob,
                result.FileDownloadName
              );
            } else {
              var url = window.URL.createObjectURL(blob);
              var link = document.createElement("a");
              document.body.appendChild(link);
              link.href = url;
              link.download = result.FileDownloadName;
              link.click();
              setTimeout(function () {
                window.URL.revokeObjectURL(url);
              }, 0);
              return false;
            }
          }
        },
        (error) => {
          this.spinnerSmallComponent.hideIt();
          this.mensajeComponent.setErrorMsg(error.message);
        }
      );
  }

}
