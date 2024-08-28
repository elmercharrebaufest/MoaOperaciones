import { Component, Input, OnInit } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { CommonResponse } from '../../common/models/common-response';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { VentaSustentableService } from '../venta-sustentable.service';

@Component({
  selector: 'app-impresion-declaracion',
  templateUrl: './impresion-declaracion.component.html',
  styleUrls: ['./impresion-declaracion.component.css']
})
export class ImpresionDeclaracionComponent extends BaseComponent {

  @Input()
  pendienteProcesarUcropit = false;
  constructor(protected service: VentaSustentableService,
    protected navService: NavService,
    protected securityService: SecurityService,
    protected sessionDataService: SessionDataService,
    protected floatMessage: FloatMsgService,
    protected modalService: ModalService
  ) {
    super(navService, securityService, floatMessage, modalService);
  }

  @Input() proveedorId: number = 0;
  @Input() cosechaId: number = 0;
  @Input() CUIT: string = "";

  descargando: boolean = false;
  texto: string = "Imprimir declaración";
  clase: string = "btn-guardar";


  ngOnInit() {
  }

  imprimir() {

    if (this.descargando) {
      return false
    }

    this.descargando = true;
    this.actualizarTextos();
    this.subscription = this.service
      .imprimirDeclaracion(this.proveedorId, this.cosechaId, this.CUIT)
      .subscribe(
        (result: CommonResponse) => {
          if (result.error) {
            this.descargando = false;
            this.floatMessage.setErrorMsg(result.error)
            this.actualizarTextos();
          } else {
            var byteArray = new Uint8Array(result.data);
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

              this.descargando = false;
              this.actualizarTextos();
              return false;
            }
          }
        },
        () => {
          this.descargando = false;
          this.actualizarTextos();
        }
      );
  }

  actualizarTextos() {
    if (this.descargando) {
      this.texto = "Generando...";
      this.clase = "btn-cerrar";
    }
    else {
      this.texto = "Imprimir declaración";
      this.clase = "btn-guardar";
    }
  }
}
