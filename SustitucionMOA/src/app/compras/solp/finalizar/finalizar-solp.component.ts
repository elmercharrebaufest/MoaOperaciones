import {
    Component,
    EventEmitter,
    Input,
    OnInit,
    Output,
    OnChanges,
    SimpleChanges,
} from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Solp } from "../solp";

@Component({
    selector: "finalizar-solp",
    templateUrl: "./finalizar-solp.component.html",
    styleUrls: ["../../compras.component.css"],
})
export class FinalizarSolpComponent implements OnInit, OnChanges {
    @Input("locale") es: any;

    @Input()
    displayFinalizar: boolean;

    @Input()
    esTipoServicio: boolean;

    @Input()
    solpActual: Solp;

    @Input()
    selectUsuarioCompras: any;

    @Input()
    usuarioComprasList: any[] = [];

    @Output() cancelarFinalizarEmitter = new EventEmitter();

    @Output() finalizarEmitter = new EventEmitter<{
        selectUsuarioCompras: any;
        solpActual: Solp;
    }>();

    certificacionAutomaticaOptions = [
        { name: "No", value: "false" },
        { name: "Si", value: "true" },
    ];

    admiteCertificacionesParcialesOptions = [
        { name: "No", value: "false" },
        { name: "Si", value: "true" },
    ];
    certificacionAutomaticaValue = {
        name: "No",
        value: "false",
    };

    admiteCertificacionesParcialesValue = {
        name: "No",
        value: "false",
    };

    disableCertificacionAutomatica = false;
    disableAdmiteCertificacionesParciales = false;

    constructor() {}

    // State OnChange para ver cuando cambie la solpActual y con esto correr la logica del ngOnInit()
    ngOnChanges(changes: SimpleChanges) {
        if (
            changes["displayFinalizar"] &&
            changes["displayFinalizar"].currentValue
        ) {
            this.displayFinalizar = changes["displayFinalizar"].currentValue;
            // Llamar a ngOnInit para aplicar la lógica de inicialización
            this.ngOnInit();
        }
    }

    ngOnInit() {

        // Verificar que solpActual existe antes de proceder
        if (!this.solpActual) {
            return;
        }

        if (this.verificarAcuerdoMarco() && this.verificarSiEsTrabajoHecho()) {
            // Usar los valores existentes de solpActual o establecer valores por defecto para trabajo hecho
            const certificacionAuto =
                this.solpActual.certificacionAutomatica !== undefined
                    ? this.solpActual.certificacionAutomatica
                    : true; // Por defecto true para trabajo hecho

            const certificacionParcial =
                this.solpActual.admiteCertificacionesParciales !== undefined
                    ? this.solpActual.admiteCertificacionesParciales
                    : false; // Por defecto false para trabajo hecho

            // Actualizar los valores de display según el modelo
            this.certificacionAutomaticaValue = {
                name: certificacionAuto ? "Si" : "No",
                value: certificacionAuto ? "true" : "false",
            };

            this.admiteCertificacionesParcialesValue = {
                name: certificacionParcial ? "Si" : "No",
                value: certificacionParcial ? "true" : "false",
            };

            // Actualizar el modelo si es necesario
            this.solpActual.certificacionAutomatica = certificacionAuto;
            this.solpActual.admiteCertificacionesParciales =
                certificacionParcial;

            // Deshabilitar controles para trabajo hecho
            this.disableCertificacionAutomatica = true;
            this.disableAdmiteCertificacionesParciales = true;
        } else {
            // Usar los valores existentes de solpActual o establecer valores por defecto normales
            const certificacionAuto =
                this.solpActual.certificacionAutomatica !== undefined
                    ? this.solpActual.certificacionAutomatica
                    : false; // Por defecto false para casos normales

            const certificacionParcial =
                this.solpActual.admiteCertificacionesParciales !== undefined
                    ? this.solpActual.admiteCertificacionesParciales
                    : false; // Por defecto false para casos normales

            // Actualizar los valores de display según el modelo
            this.certificacionAutomaticaValue = {
                name: certificacionAuto ? "Si" : "No",
                value: certificacionAuto ? "true" : "false",
            };

            this.admiteCertificacionesParcialesValue = {
                name: certificacionParcial ? "Si" : "No",
                value: certificacionParcial ? "true" : "false",
            };

            // Actualizar el modelo
            this.solpActual.certificacionAutomatica = certificacionAuto;
            this.solpActual.admiteCertificacionesParciales =
                certificacionParcial;

            // Habilitar controles para casos normales
            this.disableCertificacionAutomatica = false;
            this.disableAdmiteCertificacionesParciales = false;
        }
    }

    onCancelarFinalizar() {
        this.cancelarFinalizarEmitter.next();
    }

    puedeConfigurarCertificacionAutomatica(): boolean {
        if (this.solpActual.selectTipoPosicion) {
            const esMaterial =
                this.solpActual.selectTipoPosicion.Codigo === "MATERIALES";
            if (esMaterial) {
                return false;
            } else {
                if (this.verificarAcuerdoMarco()) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }

    puedeConfigurarCertificacionesParciales(): boolean {
        if (this.verificarAcuerdoMarco() && !this.solpActual.certificacionAutomatica) {
            return true;
        } else {
            return false;
        }
    }

    onCertificacionAutomaticaChange(event: any) {
        const valor = event.value;
        if (valor.value === "true") {
            this.solpActual.certificacionAutomatica = true;
        } else {
            this.solpActual.certificacionAutomatica = false;
        }
    }

    onAdmiteCertificacionesParcialesChange(event: any) {
        const valor = event.value;
        if (valor.value === "true") {
            this.solpActual.admiteCertificacionesParciales = true;
        } else {
            this.solpActual.admiteCertificacionesParciales = false;
        }
    }

    verificarAcuerdoMarco() {
        const acuerdoMarco = this.solpActual.posiciones.some(
            (x) => x.numeroContratoSuperior
        );
        return acuerdoMarco;
    }

    verificarSiEsTrabajoHecho() {
        const esTrabajoHecho = this.solpActual.trabajoHecho;
        return esTrabajoHecho;
    }

    onFinalizar() {
        this.solpActual.MultipleFinalizado = true;
        const updatedInfo = {
            selectUsuarioCompras: this.solpActual.selectUsuarioCompras,
            solpActual: this.solpActual,
        };
        this.finalizarEmitter.next(updatedInfo);
    }

    onHideFinalizarDialog() {
        this.cancelarFinalizarEmitter.next();
    }
}
