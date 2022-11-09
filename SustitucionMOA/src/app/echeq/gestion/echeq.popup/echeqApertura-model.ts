export class EcheqApertura {
    ordenCheque: number;
    importeCheque: number; 
    porcentaje: number;

    constructor(entity: any = null) {
        if (entity != null) {
            this.ordenCheque = entity.OrdenCheque;
            this.importeCheque = entity.ImporteCheque;
        }
    }
}




