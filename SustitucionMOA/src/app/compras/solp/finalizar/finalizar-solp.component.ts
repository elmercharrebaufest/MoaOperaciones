import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Solp } from '../solp';

@Component({
    selector: 'finalizar-solp',
    templateUrl: './finalizar-solp.component.html',
    styleUrls: ['../../compras.component.css']
})
export class FinalizarSolpComponent implements OnInit {

    @Input('locale') es: any;

    @Input()
    displayFinalizar: boolean;

    @Input()
    esTipoServicio: boolean;

    @Input()
    solpActual: Solp;

    @Output() cancelarFinalizarEmitter = new EventEmitter();

    @Output() finalizarEmitter = new EventEmitter<{ selectUsuarioCompras: any, solpActual: Solp }>();

    constructor() { }

    ngOnInit() {
    }

    onCancelarFinalizar() {
        this.cancelarFinalizarEmitter.next();
    }

    onFinalizar() {
        const updatedInfo = {
            selectUsuarioCompras: this.solpActual.selectUsuarioCompras,
            solpActual: this.solpActual
        };
        this.finalizarEmitter.next(updatedInfo);
    }

    onHideFinalizarDialog() {
        this.cancelarFinalizarEmitter.next();
    }

}
