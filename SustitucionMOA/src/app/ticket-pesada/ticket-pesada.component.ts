import { Component, OnInit, ViewChild } from "@angular/core";
import { ReCaptchaComponent } from "angular2-recaptcha";
import { environment } from "../../environments/environment";
import { BaseComponent } from "../common/base-components/base-component";
import { ArchivoDescarga } from "../common/models/archivoDescarga";
import {
    ConsultaTicketPesada,
    ConsultaTicketPesadaSubproductos,
} from "../common/models/ticket-pesada/consulta-ticket-pesada";
import { FloatMsgService } from "../common/services/FloatMsgService";
import { ModalService } from "../common/services/ModalService";
import { NavService } from "../common/services/NavService";
import { SecurityService } from "../common/services/SecurityService";
import { SessionDataService } from "../common/services/SessionDataService";
import { MensajeComponent } from "../common/view-child/mensaje/mensaje.component";
import { SpinnerComponent } from "../common/view-child/spinner/spinner.component";
import { TicketPesadaService } from "./ticket-pesada.service";
import { finalize } from "rxjs/operators";

@Component({
    selector: "app-ticket-pesada",
    templateUrl: "./ticket-pesada.component.html",
    styleUrls: ["./ticket-pesada.component.css"],
})
export class TicketPesadaComponent extends BaseComponent implements OnInit {
    TicketPesada: ConsultaTicketPesada = new ConsultaTicketPesada();
    TicketPesadaSubproductos: ConsultaTicketPesadaSubproductos =
        new ConsultaTicketPesadaSubproductos();

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("recaptchaComponent")
    protected captcha: ReCaptchaComponent;

    captchaOk: any = null;
    data: Array<ArchivoDescarga> = [];
    archivoZip: ArchivoDescarga = null;
    hayDatos: boolean = false;

    granosForm: boolean = true;
    subproductosForm: boolean = false;

    constructor(
        protected service: TicketPesadaService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService
    ) {
        super(navService, securytiService, floatMsgService, modalService);

        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    ngOnInit() {}

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }

    hayError() {
        if (this.TicketPesada.NumeroCartaPorte.toString().length < 10) {
            this.mensajeComponent.setErrorMsg("Ingrese un CTG válido");
            return true;
        }

        if (this.TicketPesada.PatenteCamion.length < 5) {
            this.mensajeComponent.setErrorMsg("Ingrese una patente válida");
            return true;
        }

        if (this.TicketPesada.Mail.length > 0) {
            if (!this.validarMail(this.TicketPesada.Mail)) {
                this.mensajeComponent.setErrorMsg("Ingrese una mail válido");
                return true;
            }
        }

        if (environment.production) {
            if (this.captchaOk == null) {
                this.mensajeComponent.setErrorMsg("Debe completar el Captcha");
                return true;
            }
        }

        return false;
    }

    hayErrorSubproductos() {
        if (this.TicketPesadaSubproductos.PatenteCamion.length < 5) {
            this.mensajeComponent.setErrorMsg("Ingrese una patente válida");
            return true;
        }

        if (environment.production) {
            if (this.captchaOk == null) {
                this.mensajeComponent.setErrorMsg("Debe completar el Captcha");
                return true;
            }
        }

        return false;
    }

    validarMail(email: string) {
        const re =
            /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(String(email).toLowerCase());
    }

    nuevaConsulta() {
        this.hayDatos = false;
        this.data = [];
        this.TicketPesada.NumeroCartaPorte = "";
        this.TicketPesada.PatenteCamion = "";
    }

    buscandoDatos = false;

    enviar() {
        if (this.hayError()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.buscandoDatos = true;

        this.subscription = this.service
            .ObtenerTicketPesada(this.TicketPesada)
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
                            let archivo = new ArchivoDescarga(
                                file.Nombre,
                                file.Datos
                            );

                            if (file.Nombre.includes(".zip")) {
                                this.archivoZip = archivo;
                            } else {
                                this.data.push(archivo);
                            }
                        });
                        this.hayDatos = true;
                    }
                },
                (error) => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    // descargarArchivo(result) {
    //     var byteArray = new Uint8Array(result.FileContents);
    //     var blob = new Blob([byteArray], { type: 'application/zip' });

    //     let nombreArchivo = `TicketPesada CCPP ${this.TicketPesada.NumeroCartaPorte}.zip`
    //     var url = window.URL.createObjectURL(blob);
    //     var link = document.createElement("a");
    //     document.body.appendChild(link);
    //     link.href = url;
    //     link.download = nombreArchivo;
    //     link.click();
    //     setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);

    //     this.TicketPesada.NumeroCartaPorte = "";
    //     this.TicketPesada.PatenteCamion = "";
    // }

    descargarArchivo(archivo: ArchivoDescarga) {
        var url = window.URL.createObjectURL(archivo.blob);
        var link = document.createElement("a");
        document.body.appendChild(link);
        link.href = url;
        link.download = archivo.Nombre;
        link.click();
        setTimeout(function () {
            window.URL.revokeObjectURL(url);
        }, 0);
    }

    showGranosForm() {
        this.granosForm = true;
        this.subproductosForm = false;
    }

    showSubproductosForm() {
        this.granosForm = false;
        this.subproductosForm = true;
    }

    enviarSubproductos() {
        if (this.hayErrorSubproductos()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.buscandoDatos = true;
        
        this.subscription = this.service
            .ObtenerTicketPesadaSubproductos(this.TicketPesadaSubproductos)
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
                            let archivo = new ArchivoDescarga(
                                file.Nombre,
                                file.Datos
                            );

                            if (file.Nombre.includes(".zip")) {
                                this.archivoZip = archivo;
                            } else {
                                this.data.push(archivo);
                            }
                        });
                        this.hayDatos = true;
                    }
                },
                (error) => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }
}
