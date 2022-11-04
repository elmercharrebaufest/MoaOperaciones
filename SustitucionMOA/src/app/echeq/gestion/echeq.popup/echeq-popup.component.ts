import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EcheqDocumento } from '../echeq-contrato.model';
import { EcheqApertura } from './echeqApertura-model';
import { registerLocaleData } from '@angular/common';
import es from '@angular/common/locales/es';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
    selector: 'app-echeq-popup',
    templateUrl: './echeq-popup.component.html',
    styleUrls: ['./echeq-popup.component.css']
})
export class EcheqPopupComponent implements OnInit {

    @Input() displayAperturarEcheq: boolean;
    @Input() set echeqDocumento(value: EcheqDocumento) {
        this.documento = value;
        this.documento.listaChequesApertura;
        this.listaChequesAux = new Array<EcheqApertura>();        
        for (var i in this.documento.listaChequesApertura) {
            var item = this.documento.listaChequesApertura[i];
            this.listaChequesAux.push({
                importeCheque: item.importeCheque,
                ordenCheque: item.ordenCheque,
                porcentaje: item.porcentaje
            });
        }
        if (this.listaChequesAux.length == 0) {
            let aforo = {
                ordenCheque: 1,
                importeCheque: Number((this.documento.importeEnPesos * this.aforoConf / 100).toFixed(2)),
                porcentaje: this.aforoConf
            };

            this.listaChequesAux.push(aforo);
        }
        this.calcularPorcentajes();
    }
    @Input() aforoConf: number;
    @Input() cantidadEcheq: number;
    @Output() cancelarAperturarEcheqEmitter = new EventEmitter();
    @Output() AperturarEcheqEmitter = new EventEmitter();
    listaChequesAux: EcheqApertura[];
    documento: EcheqDocumento;
    porcentajeRestante = 0;
    pesosPendientes = 0;

    ngOnInit() {
        registerLocaleData(es);
    }

    onCancelarAperturarEcheq() {
        this.cancelarAperturarEcheqEmitter.next();
    }

    onAperturar() {
        //validar cuando guarde que sea el 100%
        if (this.mostrarMensaje() == "") {
            this.documento.listaChequesApertura = this.listaChequesAux;
            this.AperturarEcheqEmitter.emit({
            });
        }
    }

    agregarInputEcheq() {
        this.listaChequesAux.push(
            {
                ordenCheque: this.listaChequesAux.length + 1,
                importeCheque: 0,
                porcentaje: 0
            })
    }

    eliminarEcheq(ordenCheque) {
        this.listaChequesAux.splice(ordenCheque - 1, 1);
        this.reEnumerarEcheq(this.listaChequesAux);
    }

    reEnumerarEcheq(listaChequesApertura: Array<EcheqApertura>) {
        for (let i = 1; i < listaChequesApertura.length; i++) {
            listaChequesApertura[i].ordenCheque = i + 1;
        }
    }

    calcularPorcentajes() {
        this.listaChequesAux.forEach(echeqApertura => {
            this.calcularPorcentaje(echeqApertura)
        });
    }

    calcularPorcentaje(echeqApertura: EcheqApertura) {
        echeqApertura.porcentaje = (echeqApertura.importeCheque * 100) / this.documento.importeEnPesos
    }

    //Calcular 100% ni de mas ni de menos (mensaje de aviso)
    mostrarMensaje() {
        let total = 0;
        let mensaje = "";

        this.listaChequesAux.forEach(x => {
            total += Number(x.importeCheque)
        });

        if (total != this.documento.importeEnPesos) {

            this.porcentajeRestante = 100 - total;
            this.pesosPendientes = total - this.documento.importeEnPesos;

            let formatNumber = Intl.NumberFormat('es-AR');

            if (this.pesosPendientes < 0) {
                this.pesosPendientes = this.pesosPendientes * -1;
            }

            if (this.pesosPendientes != 0) {
                if (total < this.documento.importeEnPesos) {
                    mensaje = `Tiene pendiente $${formatNumber.format(this.pesosPendientes)} por aperturar`
                } else {
                    mensaje = `Tiene exceso de $${formatNumber.format(this.pesosPendientes)} en sus Echeq`
                }
            }
        } else {
        } return mensaje;
    }

    //hacer todos los flujos
    //cuando se desmarca el cheque no se borran los echeq

}
