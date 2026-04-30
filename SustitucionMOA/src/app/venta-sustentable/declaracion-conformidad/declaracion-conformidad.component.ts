import { Component, OnInit, Output, ViewChild, EventEmitter, Input } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { VentaSustentableService } from '../venta-sustentable.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { CommonResponse } from '../../common/models/common-response';
import { DatosCopiar } from '../alta/alta.component';

@Component({
  selector: 'app-declaracion-conformidad',
  templateUrl: './declaracion-conformidad.component.html',
  styleUrls: ['./declaracion-conformidad.component.css']
})
export class DeclaracionConformidadComponent extends BaseComponent implements OnInit {

  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild("mensajeImpresion")
  protected mensajeImpresionComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  @BlockUI() blockUI: NgBlockUI;

  constructor(protected service: VentaSustentableService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMessage: FloatMsgService, protected modalService: ModalService) {
    super(navService, securityService, floatMessage, modalService);
    this.mensajeComponent = new MensajeComponent();
    this.spinnerComponent = new SpinnerComponent();
  }

  camposSustentables: any[];
  razonSocial: string = ""
  private CUIT: string = "";
  fechaActual: string = ""
  razonSocialDeclaracion: string = ""
  hectareasTotales: number = 0;
  totalidadCosecha: number = 1;
  file: File;
  esCorredor: boolean = false;
  operarComo: number = 1;
  directivaDDJJCampoSustentable: string = "";
  
  @Input() proveedorId: number = 0;
  @Input() nombreCosecha: string = "";
  @Input() cosechaId: number = 0;

  @Input() CUITDeclaracion: string = "";

  @Output() resultadoDeclaracion = new EventEmitter<boolean>();

  ngOnInit() {
  }

  verificarDeclaracion() {
    this.mensajeComponent.setMsgsEmpty();
    this.subscription = this.service.verificarDeclaracion(this.proveedorId, this.cosechaId, this.CUITDeclaracion).subscribe(
      (result: any) => {
        if (result.logout == true) {
          this.sessionDataService.logout();
        } else if (result.error != undefined && result.error != "") {
          this.mensajeComponent.setErrorMsg(result.error);
        } else if (result.info != undefined) {
          this.mensajeComponent.setInfoMsg(result.info);
        } else {

          if (!result.DeclaracionFirmada) {
            this.CUIT = result.CUIT;
            this.razonSocial = result.RazonSocial;
            this.hectareasTotales = result.HectareasDeclaracionCampoSustentable;
            this.totalidadCosecha = result.OpcionDeclaracionCampoSustentable == 0 ? 1 : 2;
            this.directivaDDJJCampoSustentable = result.DirectivaDDJJCampoSustentable;
            this.abrirModalFirmaDeclaracion()
          }
          else {
            this.resultadoDeclaracion.emit(true);
          }
        }
      },
      error => {
        this.mensajeComponent.setErrorMsg(error.message);
      }
    );
    return false;
  }

  cargarArchivo(event: any) {
    let fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      this.file = fileList[0];
    }
  }

  sizeArrayArchivoImprimido = null;
  showModalConfirmacion = false;

  imprimir() {
    this.mensajeImpresionComponent.setMsgsEmpty();

    if (!this.esCorredor || (this.esCorredor && this.operarComo == 2)) {
      this.razonSocialDeclaracion = this.razonSocialDeclaracion.trim();
      if (this.razonSocialDeclaracion.length < 3) {
        this.mensajeImpresionComponent.setErrorMsg("Debe completar la razón social.");
        return false;
      }
    }

    this.blockUI.start('Generando declaración');
    try {

      this.subscription = this.service
        .generarDeclaracionProveedor(this.proveedorId, this.cosechaId, this.hectareasTotales, this.CUITDeclaracion, this.razonSocialDeclaracion)
        .subscribe(
          (result: CommonResponse) => {
            if (result.error) {
              this.floatMessage.setErrorMsg(result.error)
            } else {
              var byteArray = new Uint8Array(result.data);
              this.sizeArrayArchivoImprimido = byteArray.byteLength;
              var blob = new Blob([byteArray], {
                type: "application/pdf",
              });
              if (window.navigator.msSaveOrOpenBlob) {
                // IE11
                window.navigator.msSaveOrOpenBlob(
                  blob,
                  "Declaracion.pdf"
                );
              } else {
                var url = window.URL.createObjectURL(blob);
                var link = document.createElement("a");
                document.body.appendChild(link);
                link.href = url;
                link.download = "Declaracion.pdf";
                link.click();
                setTimeout(function () {
                  window.URL.revokeObjectURL(url);

                }, 0);
                this.blockUI.stop();
                return false;
              }
            }
          },
          () => {
            this.floatMessage.setErrorMsg("Ocurrió un error generando la declaración")
            this.blockUI.stop();
          }
        ).add(() => {
          this.blockUI.stop();
        });;
    }
    catch (e) {
      this.floatMsgService.setErrorMsg(e);
    }
  }

  cancelar() {
    this.resultadoDeclaracion.emit(false)
    this.cerrarModal();
  }

  abrirModalFirmaDeclaracion() {
    document.getElementById("openModalHiddenButton").click();
  }

  cerrarModal() {
    document.getElementById("hidedeclaracionModal").click();
  }

  adjuntarDeclaracionFirmada() {

    if (!this.file || this.file.size < 1) {
      this.mensajeComponent.setErrorMsg("Falta adjuntar el archivo de la declaracion.");
      return true;
    }


    this.mensajeComponent.setMsgsEmpty();
    this.subscription = this.service.adjuntarDeclaracionFirmada(this.proveedorId, this.cosechaId, this.CUITDeclaracion, this.file).subscribe(
      (result: any) => {
        if (result.logout == true) {
          this.sessionDataService.logout();
        } else if (result.error != undefined && result.error != "") {
          this.mensajeComponent.setErrorMsg(result.error);
        } else if (result.info != undefined) {
          this.mensajeComponent.setInfoMsg(result.info);
        } else {
          this.mensajeComponent.setSuccessMsg(result);
          this.cerrarModal();
          this.resultadoDeclaracion.emit(true)

        }
      },
      error => {
        this.mensajeComponent.setErrorMsg(error.message);
      }
    );
    return false;
  }

  public cargarDatosCopiar(datos: DatosCopiar) {
    this.CUITDeclaracion = datos.CUIT;
    this.razonSocialDeclaracion = datos.ProveedorNombre
    this.proveedorId = datos.Proveedor_Id
    this.razonSocial = datos.ProveedorNombre
  }
}
