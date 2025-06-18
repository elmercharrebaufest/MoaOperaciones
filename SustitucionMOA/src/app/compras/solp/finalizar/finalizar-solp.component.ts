import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Solp } from '../solp';

@Component({
    selector: "finalizar-solp",
    templateUrl: "./finalizar-solp.component.html",
    styleUrls: ["../../compras.component.css"],
})
export class FinalizarSolpComponent implements OnInit {
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
        { name: "No", value: false },
        { name: "Si", value: true },
    ];

    admiteCertificacionesParcialesOptions = [
        { name: "No", value: false },
        { name: "Si", value: true },
    ];

    constructor() {}

    ngOnInit() {}

    onCancelarFinalizar() {
        this.cancelarFinalizarEmitter.next();
    }

    onCertificacionAutomaticaChange(event: any) {
        this.solpActual.certificacionAutomatica = event.value.value;
    }

    onAdmiteCertificacionesParcialesChange(event: any) {
        this.solpActual.admiteCertificacionesParciales = event.value.value;
    }

    puedeConfigurarCertificacionAutomatica(): boolean {
        if (this.solpActual.selectTipoPosicion) {
            const esMaterial = this.solpActual.selectTipoPosicion.Codigo === "MATERIALES";
            if (esMaterial) {
                return false;
            }

            const tieneContratoMarco = this.solpActual.posiciones.some((val, ind, arr) => { return val.numeroContratoSuperior; });
            return tieneContratoMarco;
        }
        return false;
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
