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
  solpActual: Solp;

  @Input()
  selectUsuarioCompras: any;

  @Input()  
  usuarioComprasList: any[] = [];

  @Output() cancelarFinalizarEmitter = new EventEmitter();

  @Output() finalizarEmitter = new EventEmitter<{solpActual: Solp}>();

  constructor() { }

  ngOnInit() {
  }

  onCancelarFinalizar() {
    this.cancelarFinalizarEmitter.next();
  }

  onFinalizar() {
    const updatedInfo = {
      //selectUsuarioCompras: this.selectUsuarioCompras, 
      solpActual: this.solpActual
    };
    this.finalizarEmitter.next(updatedInfo);
  }

  onHideFinalizarDialog() {
    this.cancelarFinalizarEmitter.next();
  }

}
