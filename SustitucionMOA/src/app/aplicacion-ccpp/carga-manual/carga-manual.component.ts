import { Component, OnInit } from "@angular/core";
import { AplicacionCcppBaseComponent } from "../aplicacion-ccpp.base.component";
import { AplicacionCcppService } from "../aplicacion-ccpp.service";
import { NavService } from "../../common/services/NavService";
import { SessionDataService } from "../../common/services/SessionDataService";
import { SecurityService } from "../../common/services/SecurityService";
import { FloatMsgService } from "../../common/services/FloatMsgService";
import { ModalService } from "../../common/services/ModalService";
import { AbstractControl, FormControl, FormGroup, Validators } from "@angular/forms";
import { AplicacionCCPPForm, CartaPorteParaAplicacionCartaPorte, ContratoParaAplicacionCartaPorte, SeccionAplicacionCCPP } from "../aplicacion-ccpp.model";
import { Subscription } from "rxjs";
import { MessageService } from "primeng/api";
import { registerLocaleData } from '@angular/common';
import es from '@angular/common/locales/es'
import { BlockUI, NgBlockUI } from "ng-block-ui";
import { debounceTime, filter } from "rxjs/operators";


export const MSG_ALERTA_CREADO = { severity: 'success', summary: 'Aplicación CCPP', detail: 'La aplicación fue guardada exitosamente.', life: 5000 };
export const MSG_ALERTA_ERROR_INFO_API = (msg: string) => ({ severity: 'error', summary: 'Aplicación CCPP', detail: msg, life: 10000 });
export const MSG_ALERTA_NO_KG_DISPONIBLES = { severity: 'warn', summary: 'Aplicación CCPP', detail: "La carta de porte seleccionada no cuenta con kg disponibles.", life: 15000 };
export const MIN_CONTRATO_LENGTH = 7;
@Component({
    styleUrls: ['carga-manual.component.css'],
    templateUrl: "carga-manual.component.html"
})
export class CargaManual extends AplicacionCcppBaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    contratos: ContratoParaAplicacionCartaPorte[] = [];
    cartasPorte: CartaPorteParaAplicacionCartaPorte[] = [];
    cartasPorteOpcion: CartaPorteParaAplicacionCartaPorte[] = [];

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
        registerLocaleData(es);
    }
    startForm() {
        this.obtenerComboContratosCartasPorte();
        this.aplicacionCCPPForm = new FormGroup({
            ContratoSeleccionado: new FormControl(null, [Validators.required,]),
            CartaPorteSeleccionada: new FormControl({ value: null, disabled: true }, [Validators.required]),
            Kilogramos: new FormControl(null, [Validators.required]),

            //Controles manuales
            Contrato: new FormControl(null, [Validators.required, Validators.minLength(7), Validators.maxLength(20)]),
            CartaPorte: new FormControl({ value: null, disabled: true }, [Validators.required, Validators.minLength(6), Validators.maxLength(12)])
        })
    }
    setTabs(): void {
        this.setMenuSeccionTab(SeccionAplicacionCCPP, 'Carga manual');
    }

    getControl(controlName: keyof AplicacionCCPPForm): AbstractControl {
        return this.aplicacionCCPPForm.get(controlName);
    }

    get contratoControl() {
        return this.getControl("Contrato");
    }
    get contratoSeleccionadoControl() {
        return this.getControl("ContratoSeleccionado");
    }
    get cartaPorteControl() {
        return this.getControl("CartaPorte");
    }
    get cartaPorteSeleccionadaControl() {
        return this.getControl("CartaPorteSeleccionada");
    }
    get kilogramosControl() {
        return this.getControl("Kilogramos");
    }

    extraOnInit(): void {
        const ctoControl = this.contratoControl;
        const ccppControl = this.cartaPorteControl;
        const contratoSeleccionadoControl = this.contratoSeleccionadoControl;
        const cartaPorteSeleccionadaControl = this.cartaPorteSeleccionadaControl;
        const kilogramosControl = this.kilogramosControl;

        this.subscriptions.add(
            contratoSeleccionadoControl
                .valueChanges.pipe(
                    filter(() => contratoSeleccionadoControl.valid)
                ).subscribe({
                    next: (contrato) => {
                        cartaPorteSeleccionadaControl.disable();
                        ccppControl.disable();
                        kilogramosControl.setValue(null);
                        this.filtrarCartasPorte(contrato.Material);
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
                        if (!cartaSeleccionada.KgPendientes) {
                            this.msgService.add(MSG_ALERTA_NO_KG_DISPONIBLES)
                        }
                        kilogramosControl.setValidators([Validators.required, Validators.min(1), Validators.max(cartaSeleccionada.KgPendientes)]);
                        kilogramosControl.setValue(cartaSeleccionada.KgPendientes)
                        kilogramosControl.updateValueAndValidity({ onlySelf: true })
                    }
                })
        )

        //Subscripciones para campos manuales
        this.subscriptions.add(
            ctoControl.valueChanges.pipe(
                debounceTime(250),
                filter(() => ctoControl.valid)
            ).subscribe((nroContrato: string) => {
                if (nroContrato.length == MIN_CONTRATO_LENGTH && !nroContrato.startsWith("0")) {
                    nroContrato = `000${nroContrato}`
                    ctoControl.setValue(nroContrato, { emitEvent: false })
                }
                const contrato = this.contratos.find(cto => cto.NumeroContrato === nroContrato)
                if (!contrato)
                    return;
                contratoSeleccionadoControl.setValue(contrato);
            })
        )
        this.subscriptions.add(
            ccppControl.valueChanges.pipe(
                filter(() => ccppControl.valid)
            ).subscribe(nroCcpp => {
                const ccpp = this.cartasPorteOpcion.find(cp => cp.NumeroCartaPorte === nroCcpp)
                if (!ccpp) {
                    kilogramosControl.setValidators([Validators.required, Validators.min(1)]);
                    kilogramosControl.setValue(null)
                    kilogramosControl.updateValueAndValidity({ onlySelf: true })
                    cartaPorteSeleccionadaControl.setValue(null)
                    return;
                }
                cartaPorteSeleccionadaControl.setValue(ccpp);
            })
        )
    }

    public extraOnDestroy(): void {
        this.subscriptions.unsubscribe();
        if (this.obtenerCartasPorteSubscription)
            this.obtenerCartasPorteSubscription.unsubscribe()
    }

    formatterContratos(data: ContratoParaAplicacionCartaPorte) {
        return data.NumeroContrato;
    }
    formatterCartaPorte(data: CartaPorteParaAplicacionCartaPorte) {
        return data.NumeroCartaPorte;
    }

    reset() {
        this.obtenerComboContratosCartasPorte();
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
                    this.msgService.add(MSG_ALERTA_ERROR_INFO_API(info || error))
                else if (data) {
                    this.msgService.add(MSG_ALERTA_CREADO)
                    this.reset()
                }
            }
        })
    }
    filtrarCartasPorte(material?: string) {
        if (!material) {
            this.cartasPorteOpcion = [...this.cartasPorte]
            return;
        }
        this.cartasPorteOpcion = this.cartasPorte.filter(ccpp => ccpp.Material === material);
        this.cartaPorteSeleccionadaControl.enable();
        //Anulacion de ccpp input manual
        this.cartaPorteControl.enable();
        //--
        this.aplicacionCCPPForm.updateValueAndValidity()
    }
    obtenerComboContratosCartasPorte() {
        this.blockUI.start();
        this.subscriptions.add(
            this.service.obtenerComboContratosCcpp().subscribe({
                next: ({ logout, info, error, data }) => {
                    this.blockUI.stop();
                    if (logout)
                        this.sessionDataService.logout()
                    else if (info || error)
                        this.msgService.add(MSG_ALERTA_ERROR_INFO_API(info || error))
                    else if (data) {
                        this.contratos = data.Contratos
                        this.cartasPorte = data.CartasPorte
                        this.filtrarCartasPorte();
                    }
                }
            })
        )
    }
}