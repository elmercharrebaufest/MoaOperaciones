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

        if(this.verificarAcuerdoMarco()){
            if(this.verificarSiEsTrabajoHecho()){
                this.solpActual.certificacionAutomatica = true;
                this.solpActual.admiteCertificacionesParciales = false;
            }
            else{
                this.solpActual.certificacionAutomatica = false;
                this.solpActual.admiteCertificacionesParciales = false;
            }
        }
    }

    onCancelarFinalizar() {
        this.cancelarFinalizarEmitter.next();
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
