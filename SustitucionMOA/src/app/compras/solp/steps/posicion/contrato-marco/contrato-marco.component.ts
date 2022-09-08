import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ContratoMarco } from './contrato-marco';
import { Solp } from '../../../solp';
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

  @Input() solpActual: Solp;

  @Output() cancelarAsociarEmitter = new EventEmitter();

  @Output() asociarEmitter = new EventEmitter();

  posicionSeleccionada: any;
  contratoSeleccionado: any;

  ngOnInit() {
  }

  onCancelarAsociar() {
    this.cancelarAsociarEmitter.next();
  }

  onAsociar() {
    this.asociarEmitter.next();
  }

  onHideAsociarDialog() {
    this.cancelarAsociarEmitter.next();
  }

}
