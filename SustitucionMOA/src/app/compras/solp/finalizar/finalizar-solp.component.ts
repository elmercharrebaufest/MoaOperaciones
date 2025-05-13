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

    constructor() {}

    ngOnInit() {}

    onCancelarFinalizar() {
        this.cancelarFinalizarEmitter.next();
    }

    onCertificacionAutomaticaChange(event: any) {
        this.solpActual.certificacionAutomatica = event.value.value;
    }

    verificarSolpConContratoMarco(): boolean {
        // Verificar que dento de solpActual y posiciones, tenga un contrato marco
        if (this.solpActual && this.solpActual.posiciones) {
            for (let i = 0; i < this.solpActual.posiciones.length; i++) {
                const pos = this.solpActual.posiciones[i];
                if (
                    pos.numeroContratoSuperior &&
                    pos.numeroContratoSuperior !== ""
                ) {
                    return true;
                }
            }
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
