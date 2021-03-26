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

@Component({
  selector: 'app-declaracion-conformidad',
  templateUrl: './declaracion-conformidad.component.html',
  styleUrls: ['./declaracion-conformidad.component.css']
})
export class DeclaracionConformidadComponent extends BaseComponent implements OnInit {

  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  constructor(protected service: VentaSustentableService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMessage: FloatMsgService, protected modalService: ModalService) {
    super(navService, securityService, floatMessage, modalService);
    this.mensajeComponent = new MensajeComponent();
    this.spinnerComponent = new SpinnerComponent();
  }

  camposSustentables: any[];
  cosechaActual: string = "";
  razonSocial: string = ""
  CUIT: string = "";
  fechaActual: string = ""
  hectareasTotales: number = 0;
  totalidadCosecha: number = 1;


  @Input() proveedorId: number = 0;

  @Output() proveedorFirmo = new EventEmitter<boolean>();


  ngOnInit() {
  }

  verificarDeclaracion() {
    this.mensajeComponent.setMsgsEmpty();
    this.subscription = this.service.verificarDeclaracion(this.proveedorId).subscribe(
      result => {
        if (result.logout == true) {
          this.sessionDataService.logout();
        } else if (result.error != undefined && result.error != "") {
          this.mensajeComponent.setErrorMsg(result.error);
        } else if (result.info != undefined) {
          this.mensajeComponent.setInfoMsg(result.info);
        } else {

          if (!result.DeclaracionFirmada) {
            this.cosechaActual = result.CosechaActual;
            this.CUIT = result.CUIT;
            this.razonSocial = result.RazonSocial;
            this.abrirModalFirmaDeclaracion()
          }
        }
      },
      error => {
        this.mensajeComponent.setErrorMsg(error.message);
      }
    );
    return false;
  }

  firmarDeclaracion() {
    if (this.totalidadCosecha == 1) {
      this.hectareasTotales = 0;
    }
    else {
      if (this.hectareasTotales <= 0) {
        this.mensajeComponent.setInfoMsg("Debe completar las hectareas totales");
        return false;
      }
    }
    this.mensajeComponent.setMsgsEmpty();
    this.subscription = this.service.firmarDeclaracion(this.proveedorId, this.hectareasTotales).subscribe(
      result => {
        if (result.logout == true) {
          this.sessionDataService.logout();
        } else if (result.error != undefined && result.error != "") {
          this.mensajeComponent.setErrorMsg(result.error);
        } else if (result.info != undefined) {
          this.mensajeComponent.setInfoMsg(result.info);
        } else {
          this.mensajeComponent.setSuccessMsg(result);

          setTimeout(() => {
            this.cerrarModal();
            this.proveedorFirmo.emit(true)
          }, 3000);
        }
      },
      error => {
        this.mensajeComponent.setErrorMsg(error.message);
      }
    );

    return false;
  }

  cancelar() {
    this.proveedorFirmo.emit(false)
    this.cerrarModal();
    this.goToSeccion('/sustentable/listado-campos');
  }

  abrirModalFirmaDeclaracion() {
    document.getElementById("openModalHiddenButton").click();
  }

  cerrarModal() {
    document.getElementById("hidedeclaracionModal").click();
  }
}
