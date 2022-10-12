import * as uuid from 'uuid';

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

    constructor(entity: any =  null) {
        if (entity != null) {
            this.contrato = entity.Contrato;
            this.pedido = entity.Pedido
            this.moneda = entity.Moneda;
            this.descripcionMaterial = entity.DescripcionMaterial;
            this.kilos = entity.Kilos;
            this.kilosPagados = entity.KilosPagados;
            this.fecha = entity.Fecha;
            this.id = uuid.v4();
            if (entity.Documentos && entity.Documentos.length) {
                this.documentos = new Array<EcheqDocumento>();
                entity.Documentos.forEach(pos => {
                    this.documentos.push(new EcheqDocumento(pos, this.id));
                });
            }
            
            if(this.pedido == ""){
                this.tipoContrato = "Fijo"
            } else {
                this.tipoContrato = "Fijación"
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

    constructor(entity: any = null, parentId: any) {
        if (entity != null) {
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
        }
    }
}



export class ObtenerContrato {
    public proveedor: string;
}