import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EcheqContrato, EcheqDocumento } from '../echeq-contrato.model';
import { EcheqApertura } from './echeqApertura-model';

@Component({
  selector: 'app-echeq-popup',
  templateUrl: './echeq-popup.component.html',
  styleUrls: ['./echeq-popup.component.css']
})
export class EcheqPopupComponent implements OnInit {

  @Input() displayAperturarEcheq: boolean;
  @Input() documento: EcheqDocumento;
 
  @Output() cancelarAperturarEcheqEmitter = new EventEmitter();

  @Output() AperturarEcheqEmitter = new EventEmitter();

  ngOnInit() {
    console.log("Documento" , this.documento);
  }

  onCancelarAperturarEcheq() {
    this.cancelarAperturarEcheqEmitter.next();
  }

  onAperturar() {
    this.AperturarEcheqEmitter.emit({
      // contrato: this.contratoSeleccionado, 
      // posicionSeleccionada:this.posicionSeleccionada
    });
  }

  onHideAperturarEcheqDialog() {
    this.cancelarAperturarEcheqEmitter.next();
  }

  agregarInputEcheq(){
    this.documento.listaChequesApertura.push(
      {
        ordenCheque: this.documento.listaChequesApertura.length + 1,
        importeCheque: 0,
        porcentaje: 0 
      })
  }

  eliminarEcheq(ordenCheque) {
    this.documento.listaChequesApertura.splice(ordenCheque - 1, 1);
    this.reEnumerarEcheq(this.documento.listaChequesApertura)
  }

  reEnumerarEcheq(listaChequesApertura: Array<EcheqApertura>) {
    for (let i = 1; i < listaChequesApertura.length; i++) {
      listaChequesApertura[i].ordenCheque = i;
    }
  }

  porcentaje: number;

  calcularPorcentaje(echeqApertura: EcheqApertura){
    let porcentaje = (echeqApertura.importeCheque * 100) / this.documento.importeEnPesos
    var intPorcentaje = Math.round( porcentaje );
    this.porcentaje = intPorcentaje;
  }

  //listaOriginal y copia lista
  //validar que no quede en 0 ni en blanco
  //Calcular 100% ni de mas ni de menos (mensaje de aviso)
  //La cantidad de cheques y el aforo tiene que consultar de la tabla de configuraciones en la db
  


}
