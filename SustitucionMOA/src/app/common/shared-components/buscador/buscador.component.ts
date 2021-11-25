import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../base-components/base-component';
import { FloatMsgService } from '../../services/FloatMsgService';
import { ModalService } from '../../services/ModalService';
import { NavService } from '../../services/NavService';
import { SecurityService } from '../../services/SecurityService';
import { SessionDataService } from '../../services/SessionDataService';
import { SpinnerComponent } from '../../view-child/spinner/spinner.component';
import { BuscadorService } from './buscador.service';
import { Resultado, ResultadoTipo } from './Buscador';
import { OverlayPanel } from 'primeng/overlaypanel';
import { SpinnerSmallComponent } from '../../view-child/spinner-small/spinner-small.component';
declare var $: any;

@Component({
  selector: 'app-buscador-inteligente',
  templateUrl: './buscador.component.html',
  styleUrls: ['./buscador.component.css'],
  providers: [BuscadorService],
})
export class BuscadorComponent extends BaseComponent implements OnInit {

  @ViewChild("spinnerModal")
  protected spinnerModalComponent: SpinnerComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  @ViewChild(SpinnerSmallComponent)
  public spinnerSmallComponent: SpinnerSmallComponent;

  constructor(protected service: BuscadorService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService) {

    super(navService, securytiService, floatMsgService, modalService);
    this.spinnerComponent = new SpinnerComponent();
    this.spinnerModalComponent = new SpinnerComponent();
    this.spinnerSmallComponent = new SpinnerSmallComponent();
  }

  palabraABuscar: string;
  palabraABuscarAuxiliar: string;
  resultados: Resultado[] = [];
  hayResultados: boolean = true;
  tipos: ResultadoTipo = new ResultadoTipo();
  tituloArchivoPDF = "Documento"

  ngOnInit() {
    var input = document.getElementById("buscador");

    input.addEventListener("keyup", function(event) {
      if (event.keyCode === 13 || $(this).val().length >= 6) {
        event.preventDefault();
        document.getElementById("autoBusqueda").click();
      }
    }); 
  }

  goToSeccionSelector(resultado: Resultado){
    switch(resultado.CtaParams){
      case 1:
        this.goToSeccionParam(resultado.Link, resultado.Value);
        break
      case 2:
        this.goToSeccionParamDos(resultado.Link, resultado.Value, resultado.Value);
        break
      default:
        this.goToSeccion(resultado.Link);
        break
    }
  }

  autoBusqueda(event, overlaypanel: OverlayPanel){
    overlaypanel.show(event);

    if(this.palabraABuscar == undefined || this.palabraABuscar != this.palabraABuscarAuxiliar || this.resultados.length < 1){
      this.resultados = [];
      this.spinnerSmallComponent.showIt();
      this.palabraABuscarAuxiliar = this.palabraABuscar;
      this.hayResultados = true;

      try {
        this.subscription = this.service.getResultados(this.palabraABuscar).subscribe(
            (result: any) => {
              if (result.logout == true) {
                this.sessionDataService.logout();
              } else{
                this.spinnerSmallComponent.hideIt();
                this.resultados = result.data;

                if(this.resultados.length >= 1){
                  this.hayResultados = true;
                }
                else{
                  this.hayResultados = false
                }
              }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );
      } catch (e) {
          this.floatMsgService.setErrorMsg(e);
          return false; //<-- Prevent Refresh
      }
    }
    else{
      return false;
    }
  }

  public switchDescargas(resultado: Resultado){
    switch (resultado.Code) {
      case this.tipos.Liquidacion:
        this.descargaPDFLiquidacion(resultado.Value);
        break;

      case this.tipos.ProformaFinal:
        debugger
        this.descargarProforma(resultado.Value);
        console.log(resultado)
        break;
    
      default:
        break;
    }
  }

  descargarProforma(fijacion: string){
    this.spinnerSmallComponent.showIt();
    this.unsubscribe();
    this.subscription = this.service.descargarProformaFinal(fijacion).subscribe(
        (result:any) => {
            this.spinnerSmallComponent.hideIt();
            if(result.pdf){
                var byteArray = new Uint8Array(result.pdf.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivoPDF + fijacion + '.pdf');
                } else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = this.tituloArchivoPDF + fijacion + '.pdf';
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
            else{
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                } else if (result.info != undefined) {
                }
            }
        },
        error => {
            this.spinnerSmallComponent.hideIt();
        }

    );
    return false;
  }

  descargaPDFLiquidacion(value: string) {
    this.floatMsgService.setMsgsEmpty();
    this.unsubscribe();
    let valoresSplit = value.split(",");
    let documento = valoresSplit[0];
    let ejercicio = valoresSplit[1]

    try {
      this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(
          (result: any) => {
              if (result.logout == true) {
                  this.sessionDataService.logout();
              } else if (result.error != undefined && result.error != "") {
                  this.floatMsgService.setErrorMsg(result.error);
              } else if (result.info != undefined) {
                  this.floatMsgService.setInfoMsg(result.info);
              } else {
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivoPDF + documento + ".pdf");
                } else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = this.tituloArchivoPDF + documento + ".pdf"
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
              }
          },
          error => {
              this.floatMsgService.setErrorMsg(error.message);
          }

      );
    } catch (e) {
        this.floatMsgService.setErrorMsg(e);
        return false; //<-- Prevent Refresh
    }
  }

}
