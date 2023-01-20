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
        if(value != undefined && value != null ){
            this.documento = value;
            // this.documento.listaChequesApertura;
            this.listaChequesAux = new Array<EcheqApertura>();        
            for (var i in this.documento.listaChequesApertura) {
                var item = this.documento.listaChequesApertura[i];
                this.listaChequesAux.push({
                    importeCheque: item.importeCheque,
                    ordenCheque: item.ordenCheque,
                    porcentaje: item.porcentaje
                });
            }
            let formatNumber = Intl.NumberFormat('es-AR');

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
    }
    @Input() aforoConf: number;
    @Input() cantidadEcheq: number;
    @Output() cancelarAperturarEcheqEmitter = new EventEmitter();
    @Output() AperturarEcheqEmitter = new EventEmitter();
    listaChequesAux: EcheqApertura[] = new Array<EcheqApertura>();
    documento: EcheqDocumento;
    porcentajeRestante = 0;
    pesosPendientes = 0;

    condicionBoton: boolean = true;
    mensajeMontosValidacion: string = "";
    mensajeRecordatorio: string = "";

    ngOnInit() {
        registerLocaleData(es);
    }

    onCancelarAperturarEcheq() {
        this.cancelarAperturarEcheqEmitter.next();
    }

    onAperturar() {
        //validar cuando guarde que sea el 100%
        if(!this.validarEcheqVacio()){
            this.mensajeRecordatorio = "Tiene campos obligatorios sin completar"

        } else if (this.mensajeMontosValidacion == "") {
            this.documento.listaChequesApertura = this.listaChequesAux;
            this.AperturarEcheqEmitter.emit({
            });
            this.mensajeRecordatorio = "";
        } else {
            this.mensajeRecordatorio = "Recuerde que aún tiene saldo pendiente de aperturar"

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
        this.mostrarMensaje();
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
        echeqApertura.porcentaje = (echeqApertura.importeCheque * 100) / this.documento.importeEnPesos;
        this.mostrarMensaje();
    }

    //Calcular 100% ni de mas ni de menos (mensaje de aviso)
    mostrarMensaje() {
        let total = 0;
        this.mensajeMontosValidacion = "";

        this.listaChequesAux.forEach(x => {
            total += Number(x.importeCheque)
        });

        total = Number(total.toFixed(2));

        if (total != this.documento.importeEnPesos) {
            this.condicionBoton = true;

            this.porcentajeRestante = 100 - total;
            this.pesosPendientes = total - this.documento.importeEnPesos;

            let formatNumber = Intl.NumberFormat('es-AR');

            if (this.pesosPendientes < 0) {
                this.pesosPendientes = this.pesosPendientes * -1;
            }

            if (this.pesosPendientes != 0) {
                if (total < this.documento.importeEnPesos) {
                    this.mensajeMontosValidacion = `Tiene pendiente $${formatNumber.format(this.pesosPendientes)} por aperturar`
                } else {
                    this.mensajeMontosValidacion = `Tiene exceso de $${formatNumber.format(this.pesosPendientes)} en sus Echeq`
                }
            }
        } else {
            this.condicionBoton = false;
            this.mensajeRecordatorio = "";
        } 
    }

    validarEcheqVacio(){
        debugger
        return this.listaChequesAux.every(x => x.importeCheque > 1 );
    }

}
