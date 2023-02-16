import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { ContratoMarco } from './contrato-marco';
import { SolpPosicion } from '../../../solp-posicion';
import { SelectItem } from 'primeng/api';
import { Dropdown } from 'primeng/dropdown';

@Component({
  selector: 'asociar-contrato-marco',
  templateUrl: './contrato-marco.component.html',
  styleUrls: ['./contrato-marco.component.css']
})

export class ContratoMarcoComponent implements OnChanges {

  @Input() nroSolp: string;

  @Input() displayAsociar: boolean;

  @Input() listaContratos: ContratoMarco[];

  @Input() listaDePosicionesAsociar: any[];

  @Input() posiciones: SolpPosicion[];

  @Output() cancelarAsociarEmitter = new EventEmitter();

  @Output() asociarEmitter = new EventEmitter();

  posicionSeleccionada: SolpPosicion;
  contratoSeleccionado: any = null;
  posicionesCombo: SelectItem[];
  posicionSelect: any;
  contratos: any;
  filterValue: string;

  ngOnChanges() {
    this.getComboPosiciones();
  }

  onChangeContrato() {
    this.posicionSeleccionada = this.posiciones.find(posicion => posicion.numeroPosicion == this.posicionSelect.value);
  }

  onCancelarAsociar() {
    this.cancelarAsociarEmitter.next();
  }

  onAsociar(dd: Dropdown) {
    this.asociarEmitter.emit({
      contrato: this.contratoSeleccionado,
      posicionSeleccionada: this.posicionSeleccionada
    });
    this.onHideAsociarDialog(dd);
  }

  onHideAsociarDialog(dropdown: Dropdown) {
    this.cancelarAsociarEmitter.next();
    this.clearDropdown(dropdown);
    this.contratos = [];
  }

  getContratoMarco() {    
    if (this.listaDePosicionesAsociar != undefined) {
      this.contratos = this.listaDePosicionesAsociar.filter(x => x.Indice == this.posicionSelect.value)[0].ContratosAsociados;
      var proveedor = this.listaDePosicionesAsociar.filter(x => x.Indice == this.posicionSelect.value)[0].Proveedor;
      if(proveedor){
        this.contratoSeleccionado = this.contratos.filter(x => x.ProveedorFijo == proveedor)[0];
      }
    }
    this.onChangeContrato();
  }

  getComboPosiciones() {
    if (this.listaDePosicionesAsociar != undefined) {
      this.posicionesCombo = this.listaDePosicionesAsociar.map(x => ({
        label: x.Indice + " - " + x.Tarea,
        value: x.Indice
      }));     
    }
  }

  clearDropdown(dropdown: Dropdown) {
    this.posicionSelect.value = null;
    dropdown.resetFilter();
}

}
