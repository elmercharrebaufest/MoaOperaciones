import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ContratoMarco } from './contrato-marco';
import { SolpPosicion } from '../../../solp-posicion';

@Component({
  selector: 'asociar-contrato-marco',
  templateUrl: './contrato-marco.component.html',
  styleUrls: ['./contrato-marco.component.css']
})

export class ContratoMarcoComponent implements OnInit {

  @Input() nroSolp: string;

  @Input() displayAsociar: boolean;

  @Input() listaContratos: ContratoMarco[];

  @Input() posiciones: SolpPosicion[];

  @Output() cancelarAsociarEmitter = new EventEmitter();

  @Output() asociarEmitter = new EventEmitter();

  posicionSeleccionada: SolpPosicion;
  contratoSeleccionado: any = null;

  ngOnInit() {}

  onChangeContrato(posicionActual) {
    this.posicionSeleccionada = this.posiciones.find(posicion => posicion.id == posicionActual.id);
  }

  onCancelarAsociar() {
    this.cancelarAsociarEmitter.next();
  }

  onAsociar() {
    this.asociarEmitter.emit({
      contrato: this.contratoSeleccionado, 
      posicionSeleccionada:this.posicionSeleccionada
    });
  }

  onHideAsociarDialog() {
    this.cancelarAsociarEmitter.next();
  }

}
