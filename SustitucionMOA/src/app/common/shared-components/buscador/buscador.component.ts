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
import { MensajeComponent } from '../../view-child/mensaje/mensaje.component';
import { Message, MessageService } from 'primeng/api';

declare var $: any;

@Component({
  selector: 'app-buscador-inteligente',
  templateUrl: './buscador.component.html',
  styleUrls: ['./buscador.component.css'],
  providers: [MessageService],
})
export class BuscadorComponent extends BaseComponent implements OnInit {

  @ViewChild("spinnerModal")
  protected spinnerModalComponent: SpinnerComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  @ViewChild(SpinnerSmallComponent)
  public spinnerSmallComponent: SpinnerSmallComponent;

  constructor(protected readonly service: BuscadorService, protected navService: NavService,
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
    this.unsubscribe();

    if(this.service.palabraABuscar == undefined || this.service.palabraABuscar != this.palabraABuscarAuxiliar || this.resultados.length < 1){
      this.resultados = [];
      this.spinnerSmallComponent.showIt();
      this.palabraABuscarAuxiliar = this.service.palabraABuscar;
      this.hayResultados = true;

      try {
        this.subscription = this.service.getResultados(this.service.palabraABuscar).subscribe(
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

  public descargarComprobante(resultado: Resultado = null, value: string = ""){
    switch (resultado.Code) {
      case this.tipos.Liquidacion:
        this.descargaPDFLiquidacion(resultado.Value);
        break;

      case this.tipos.ProformaFinalAgrupador:
        this.descargarProforma(value);
        break;

      case this.tipos.CartaPorte:
        this.descargarFotosCCPP(resultado.Value)
    
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
            if(result.Pdf){
                var byteArray = new Uint8Array(result.Pdf.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                var url = window.URL.createObjectURL(blob);
                var link = document.createElement("a");
                document.body.appendChild(link);
                link.href = url;
                link.download = this.tituloArchivoPDF + fijacion + '.pdf';
                link.click();
                setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                return false;
            }
            else{
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                  this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                  this.floatMsgService.setInfoMsg(result.info);
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

      if (documento.split("|").length == 1) {
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
                          var url = window.URL.createObjectURL(blob);
                          var link = document.createElement("a");
                          document.body.appendChild(link);
                          link.href = url;
                          link.download = this.tituloArchivoPDF + documento + ".pdf"
                          link.click();
                          setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                          return false;
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
      } else {
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
                          var byteArray = new Uint8Array(result.FileContents);
                          var blob = new Blob([byteArray], { type: 'application/zip' });
                          var url = window.URL.createObjectURL(blob);
                          var link = document.createElement("a");
                          document.body.appendChild(link);
                          link.href = url;
                          link.download = "Liquidaciones_" + ejercicio+".zip";
                          link.click();
                          setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                          return false;
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

  descargarFotosCCPP(cartaPorteIDStr: string) {
    this.spinnerSmallComponent.showIt();
    this.floatMsgService.setMsgsEmpty
    this.unsubscribe();
    this.subscription = this.service.descargarFotosCCPP(cartaPorteIDStr).subscribe(
        (result:any) => {
            this.spinnerSmallComponent.hideIt();
            if (result.logout == true) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.floatMsgService.setErrorMsg(result.error)
            } else if (result.info != undefined) {
              this.floatMsgService.setInfoMsg(result.info)
            } else {
                var byteArray = new Uint8Array(result.FileContents);
                var blob = new Blob([byteArray], { type: 'application/zip' });
                var url = window.URL.createObjectURL(blob);
                var link = document.createElement("a");
                document.body.appendChild(link);
                link.href = url;
                link.download = this.tituloArchivoPDF;
                link.click();
                setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                return false;
            }
        },
        error => {
            this.spinnerSmallComponent.hideIt();
        }
    );
    return false;  // <- Prevent href del a
  }

}
