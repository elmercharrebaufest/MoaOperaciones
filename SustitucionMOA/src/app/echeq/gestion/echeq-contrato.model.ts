import * as uuid from 'uuid';
import { EcheqApertura } from './echeq.popup/echeqApertura-model';

export class EcheqContrato {

    public id: any;
    public selected: boolean;
    public contrato: string;
    public pedido: string;
    public moneda: string;
    public descripcionMaterial: string;
    public kilos: number;
    public kilosPagados: number;
    public fecha: string;
    public documentos: Array<EcheqDocumento> = new Array<EcheqDocumento>();
    public tipoContrato: string;
    public expanded: boolean;
    public clasificacion: string;

    constructor(entity: any =  null) {
        if (entity != null) {
            this.selected = entity.MarcaCheque;
            this.contrato = entity.Contrato;
            this.pedido = entity.Pedido
            this.moneda = entity.Moneda;
            this.descripcionMaterial = entity.DescripcionMaterial;
            this.kilos = entity.Kilos;
            this.kilosPagados = entity.KilosPagados;
            this.fecha = entity.Fecha;
            this.tipoContrato = entity.TipoContrato;
            this.id = uuid.v4();
            this.clasificacion = entity.Clasificacion;
            if (entity.Documentos && entity.Documentos.length) {
                this.documentos = new Array<EcheqDocumento>();
                entity.Documentos.forEach(pos => {
                    this.documentos.push(new EcheqDocumento(pos, this.id));
                });
            }
        }
    }
}




export class EcheqDocumento {

    public parentId: any;
    public selected: boolean;
    public contrato: string;
    public pedido: string;
    public sociedad: string;
    public documento: string;
    public ejercicio: string;
    public fecha: string;
    public numeroCOE: string;
    public solapa: number;
    public importeMonedaDocumento: string;
    public importeEnPesos: number;
    public moneda: number;
    public clasificacion: string;
    public listaChequesApertura: EcheqApertura[];

    constructor(entity: any = null, parentId: any) {
        if (entity != null) {
            this.selected = entity.MarcaCheque;
            this.parentId = parentId;
            this.contrato = entity.Contrato;
            this.pedido = entity.Pedido;
            this.sociedad = entity.Sociedad;
            this.documento = entity.Documento;
            this.ejercicio = entity.Ejercicio;
            this.fecha = entity.Fecha;
            this.numeroCOE = entity.NumeroCOE;
            this.solapa = entity.Solapa;
            this.importeMonedaDocumento = entity.ImporteMonedaDocumento;
            this.importeEnPesos = entity.ImporteEnPesos;
            this.moneda = entity.Moneda;
            this.clasificacion = entity.Clasificacion;
            if (entity.Aperturas && entity.Aperturas.length) {
                this.listaChequesApertura = new Array<EcheqApertura>();
                entity.Aperturas.forEach(ape => {
                    this.listaChequesApertura.push(new EcheqApertura(ape));
                });
            }
        } else {
            this.listaChequesApertura = new Array<EcheqApertura>();
        }
    }


    aperturas() {
        let result = "";
        let formatNumber = Intl.NumberFormat('es-AR');
        if (this.listaChequesApertura.length > 0) {
            this.listaChequesApertura.sort((a, b) => a.ordenCheque - b.ordenCheque).forEach(ape => {
                result += `Echeq ${ape.ordenCheque}: $${formatNumber.format(ape.importeCheque)}<br>`;
            });
        }
        return result;
    }
}

export enum ClasificacionEcheq {
    Productor="PRODUCTOR",
    Acopiador="ACOPIADOR"
}

export class ObtenerContrato {
    public proveedor: string;
}