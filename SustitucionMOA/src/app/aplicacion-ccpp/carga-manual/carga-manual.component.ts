import { Component, OnInit } from "@angular/core";
import { AplicacionCcppBaseComponent } from "../aplicacion-ccpp.base.component";
import { AplicacionCcppService } from "../aplicacion-ccpp.service";
import { NavService } from "../../common/services/NavService";
import { SessionDataService } from "../../common/services/SessionDataService";
import { SecurityService } from "../../common/services/SecurityService";
import { FloatMsgService } from "../../common/services/FloatMsgService";
import { ModalService } from "../../common/services/ModalService";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { CartaPorteParaAplicacionCartaPorte, ContratoParaAplicacionCartaPorte, SeccionAplicacionCCPP } from "../aplicacion-ccpp.model";
import { Subscription } from "rxjs";
import { debounceTime, filter } from "rxjs/operators";
import { MessageService } from "primeng/api";

export const MSG_ALERTA_CREADO = { severity: 'success', summary: 'Aplicación CCPP', detail: 'La aplicación fue guardada exitosamente.', life: 5000 };
export const MSG_ALERTA_NO_CREADO = (msg: string) => ({ severity: 'error', summary: 'Aplicación CCPP', detail: msg, life: 10000 });

@Component({
    styleUrls: ['carga-manual.component.css'],
    templateUrl: "carga-manual.component.html"
})
export class CargaManual extends AplicacionCcppBaseComponent implements OnInit {

    contratos: ContratoParaAplicacionCartaPorte[] = [];
    cartasPorte: CartaPorteParaAplicacionCartaPorte[] = [];

    aplicacionCCPPForm: FormGroup;
    subscriptions = new Subscription();
    obtenerCartasPorteSubscription?: Subscription;

    constructor(
        protected service: AplicacionCcppService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        protected msgService: MessageService
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.startForm()
    }
    startForm() {
        this.aplicacionCCPPForm = new FormGroup({
            Contrato: new FormControl(null, [Validators.required, Validators.minLength(6), Validators.maxLength(10)]),
            CartaPorteSeleccionada: new FormControl({ value: null, disabled: true }, [Validators.required, Validators.minLength(6), Validators.maxLength(10)]),
            Kilogramos: new FormControl(null, [Validators.required]),
        })
    }
    setTabs(): void {
        this.setMenuSeccionTab(SeccionAplicacionCCPP, 'Carga manual');
    }

    extraOnInit(): void {
        const contratoControl = this.aplicacionCCPPForm.get("Contrato");
        const cartaPorteSeleccionadaControl = this.aplicacionCCPPForm.get("CartaPorteSeleccionada");
        const kilogramosControl = this.aplicacionCCPPForm.get("Kilogramos");
        this.subscriptions.add(
            this.service.obtenerContratos().subscribe({
                next: (res) => this.contratos = res.data.Contratos
            })
        )
        this.subscriptions.add(
            contratoControl
                .valueChanges.pipe(
                    debounceTime(500),
                    filter(() => contratoControl.valid)
                ).subscribe({
                    next: (contrato) => {
                        if (typeof (contrato) !== "string") {
                            contrato = contrato.NumeroContrato;
                            contratoControl.setValue(contrato, { emitEvent: false })
                        }
                        cartaPorteSeleccionadaControl.disable();
                        kilogramosControl.setValue(null);
                        this.obtenerCartasPorteSubscription = this.service
                            .obtenerCartasPorte(contrato)
                            .subscribe({
                                next: (res) => {
                                    this.cartasPorte = res.data.CartasPorte
                                    cartaPorteSeleccionadaControl.enable();
                                    this.aplicacionCCPPForm.updateValueAndValidity()
                                }
                            })
                    },
                }),
        )
        this.subscriptions.add(
            cartaPorteSeleccionadaControl.valueChanges
                .pipe(
                    filter(() => cartaPorteSeleccionadaControl.valid)
                )
                .subscribe({
                    next: (cartaSeleccionada) => {
                        kilogramosControl.setValidators([Validators.required, Validators.min(1), Validators.max(cartaSeleccionada.KgPendientes)]);
                        kilogramosControl.setValue(cartaSeleccionada.KgPendientes)
                        kilogramosControl.updateValueAndValidity({ onlySelf: true })
                    }
                })
        )
    }

    public extraOnDestroy(): void {
        this.subscriptions.unsubscribe();
        if (this.obtenerCartasPorteSubscription)
            this.obtenerCartasPorteSubscription.unsubscribe()
    }

    formatterContratos(data: ContratoParaAplicacionCartaPorte): string {
        return data.NumeroContrato;
    }

    reset() {
        this.aplicacionCCPPForm.reset()
    }

    guardarAplicacion() {
        if (!this.aplicacionCCPPForm.valid)
            return;
        this.service.guardarAplicacion(this.aplicacionCCPPForm.value).subscribe({
            next: ({ data, info, error, logout }) => {
                if (logout)
                    this.sessionDataService.logout()
                else if (info || error)
                    this.msgService.add(MSG_ALERTA_NO_CREADO(info || error))
                else if (data) {
                    this.msgService.add(MSG_ALERTA_CREADO)
                    this.reset()
                }
            }
        })
    }
}