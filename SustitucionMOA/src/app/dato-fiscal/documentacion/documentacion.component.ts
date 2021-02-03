import { Component, OnInit } from "@angular/core";
import { DatoFiscalService } from "./../dato-fiscal.service";
import { NavService } from "./../../common/services/NavService";
import { FloatMsgService } from "./../../common/services/FloatMsgService";
import { SessionDataService } from "./../../common/services/SessionDataService";
import { SecurityService } from "./../../common/services/SecurityService";
import { Seccion } from "./../../common/models/seccion";
import { BaseComponent } from "./../../common/base-components/base-component";
import { ModalService } from "./../../common/services/ModalService";

@Component({
  selector: "documentacion",
  templateUrl: `documentacion.component.html`,
  providers: [DatoFiscalService],
})
export class DocumentacionComponent extends BaseComponent implements OnInit {
  constructor(
    protected service: DatoFiscalService,
    protected navService: NavService,
    protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService,
    protected modalService: ModalService,
    private sessionDataService: SessionDataService,
  ) {
    super(navService, securityService, floatMsgService, modalService);
    this.isGranosSelected = sessionStorage.getItem("granosSelected");
    sessionDataService.granosSelected$.subscribe(
        granosSelected => {
            this.isGranosSelected = granosSelected;
        });
  }

  tipoUsuario: string;
  isGranosSelected: string;

  setTabs() {
    this.setMenuSeccionTab("dato-fiscal", "Documentacion");
  }

  ngOnInit() {
    this.setTabs();
    var secciones = [];
    this.navService.setSeccionList(secciones);
  }

  isGranos() {
    return this.isGranosSelected == "G";
  }

  // Se vuelve a la solucion de tener la documentacion dentro del proyecto
  /*getDocumento(nombre:string) {
        this.unsubscribe();
        try {
            this.subscription = this.service.getDocumento(nombre).subscribe(
                result => {
                    //this.spinnerComponent.hideIt();
                    var tipoArchivo = 'application/octet-stream';
                    try {
                        var res = nombre.split(".");
                        var extension = res[res.length - 1];
                        if (extension.toLocaleUpperCase() == "PDF") {
                            tipoArchivo = 'application/pdf';
                        }
                    }
                    catch{ }
                    if (result.logout == true) {
                        //this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                       //this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        //this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        var byteArray = new Uint8Array(result.documento);
                        var blob = new Blob([byteArray], { type: tipoArchivo });
                        if (window.navigator.msSaveOrOpenBlob) {
                            //IE11
                            window.navigator.msSaveOrOpenBlob(blob, nombre);
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = nombre;
                            link.click();
                            setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        }
                    }
                },
                error => {
                    //this.spinnerComponent.hideIt();
                    //this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            //this.spinnerComponent.hideIt();
            //this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false;
    }*/
}
