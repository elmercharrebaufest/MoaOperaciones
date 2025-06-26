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
        console.log("ngOnChanges ejecutado con cambios:", changes);
        if (changes["displayFinalizar"] && changes["displayFinalizar"].currentValue) {
            console.log("Ejecuta ngOnChanges de FinalizarSolpComponent");
            this.displayFinalizar = changes["displayFinalizar"].currentValue;
            // Llamar a ngOnInit para aplicar la lógica de inicialización
            this.ngOnInit();
        }
    }

    ngOnInit() {
        console.log("Ejecuta ngOnInit de FinalizarSolpComponent");
        if (this.verificarAcuerdoMarco() && this.verificarSiEsTrabajoHecho()) {
            console.log("Puede configurar certificación automática.");
            this.certificacionAutomaticaValue = {
                name: "Si",
                value: "true",
            };
            this.admiteCertificacionesParcialesValue = {
                name: "No",
                value: "false",
            };
            this.solpActual.certificacionAutomatica = true;
            this.solpActual.admiteCertificacionesParciales = false;
            this.disableCertificacionAutomatica = true;
            this.disableAdmiteCertificacionesParciales = true;
        } else {
            this.certificacionAutomaticaValue = {
                name: "No",
                value: "false",
            };
            this.admiteCertificacionesParcialesValue = {
                name: "No",
                value: "false",
            };
            this.solpActual.certificacionAutomatica = false;
            this.solpActual.admiteCertificacionesParciales = false;
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
                return true;
            }
        }
    }

    onCertificacionAutomaticaChange(event: any) {
        const valor = event.value;
        console.log("Valor de certificación automática:", valor);
        if (valor.value === "true") {
            this.solpActual.certificacionAutomatica = true;
        } else {
            this.solpActual.certificacionAutomatica = false;
        }
        console.log(
            "Solp actual certificación automática:",
            this.solpActual.certificacionAutomatica
        );
    }

    onAdmiteCertificacionesParcialesChange(event: any) {
        const valor = event.value;
        console.log("Valor de certificaciones parciales:", valor);
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
