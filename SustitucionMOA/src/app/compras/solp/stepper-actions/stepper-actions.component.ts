import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Paso } from '../../../common/models/paso';
import { Solp } from '../solp';

@Component({
    selector: 'stepper-actions',
    templateUrl: './stepper-actions.component.html',
    styleUrls: ['../../compras.component.css']
})
export class StepperActionsComponent implements OnInit {

    @Input()
    disabledSave: boolean;

    @Input()
    saveButtonAvailable: boolean;

    @Input()
    saveButtonPliegoMultipleAvailable: boolean;

    @Input()
    solpActual: Solp;

    @Input()
    esAuditor: boolean;

    @Input()
    pasos: Paso[];

    @Input()
    pasoActual: Paso;

    @Input()
    tipoSolp: string;

    @Output() cancelarSolpEmitter = new EventEmitter();

    @Output() navegarEmitter = new EventEmitter<any>();

    @Output() guardarCambiosEmitter = new EventEmitter<any>();

    @Output() showFinalizarDialogEmitter = new EventEmitter();

    @Output() previewEmitter = new EventEmitter();

    @Output() finalizarEmitter = new EventEmitter<{ selectUsuarioCompras: any, solpActual: Solp }>();


    constructor() { }

    ngOnInit() {
    }

    pasoAnterior() {
        if (this.pasoActual.Numero == 1) {
            return { paso: null, descripcion: 'VOLVER' };
        } else {
            if (this.tipoSolp === 'SIN_PLIEGO' && this.pasoActual.Numero == 5) {
                var prev = this.pasos.find(x => x.Numero == 4);
                return { paso: prev, descripcion: `PASO ${prev.Numero}` }
            }
            if (this.tipoSolp === 'SIN_PLIEGO' && this.pasoActual.Numero == 4) {
                var prev = this.pasos.find(x => x.Numero == 2);
                return { paso: prev, descripcion: `PASO ${prev.Numero}` }
            }

            if (this.tipoSolp === 'SIN_PLIEGO' && this.pasoActual.Numero == 2) {
                var prev = this.pasos.find(x => x.Numero == 2);
                return { paso: null, descripcion: 'VOLVER' };
            }
            var prev = this.pasos.find(x => x.Numero == this.pasoActual.Numero - 1);

            return { paso: prev, descripcion: `PASO ${prev.Numero}` }
        }
    }

    pasoSiguiente() {
        if (this.pasoActual.Numero == this.pasos[this.pasos.length - 1].Numero) {
            return { paso: null, descripcion: 'FINALIZAR' };
        } else {
            if (this.tipoSolp === 'SIN_PLIEGO' && this.pasoActual.Numero == 2) {
                var next = this.pasos.find(x => x.Numero == 4);
                return { paso: next, descripcion: `PASO ${next.Numero}` }
            }

            var next = this.pasos.find(x => x.Numero == this.pasoActual.Numero + 1);

            return { paso: next, descripcion: `PASO ${next.Numero}` }
        }
    }

    onCancelarSolp() {
        this.cancelarSolpEmitter.next();
    }

    onPasoAnterior() {
        this.navegarEmitter.next(this.pasoAnterior());
        const params = {
            mostrarPreview: false,
            enviarSap: false,
            guardarPorPaso: true
        };
        if (!this.esAuditor) {
            this.guardarCambiosEmitter.next(params);
        }
    }

    onPasoSiguiente() {
        this.navegarEmitter.next(this.pasoSiguiente());
        const params = {
            mostrarPreview: false,
            enviarSap: false,
            guardarPorPaso: true
        };
        if (!this.esAuditor && this.puedeGuardarSolp()) {
            this.guardarCambiosEmitter.next(params);
        }
    }

    onGuardar() {
        const params = {
            mostrarPreview: false,
            enviarSap: false,
            guardarPorPaso: false
        };
        if (!this.esAuditor) {
            this.guardarCambiosEmitter.next(params);
        }
    }

    onGuardarPliegoMultiple(){
        this.solpActual.revisadoPor="-";
        const updatedInfo = {
            selectUsuarioCompras: this.solpActual.selectUsuarioCompras,
            solpActual: this.solpActual
        };
        this.finalizarEmitter.next(updatedInfo);
    }

    onShowFinalizarDialog() {
        if (!this.esAuditor) {
            this.showFinalizarDialogEmitter.next();
        }
    }

    onPreview() {
        this.previewEmitter.next();
    }

    puedeGuardarSolp(): boolean {
        if (this.solpActual.esSolpDeCompras() && this.solpActual.ordenesDeCompraGeneradas.length > 0) {
            return false;
        }
        return true;
    }
}
