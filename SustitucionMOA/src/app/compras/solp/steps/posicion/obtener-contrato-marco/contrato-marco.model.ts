export class ContratoMarco {
    public numeroDocumentoCompras: string;
    public sociedad: string;
    public indicadorDeBorrado: string;
    public numeroCuentaProveedor: string;
    public nombreProveedor: string;
    public organizacionCompras: string;
    public grupoCompras: string;
    public claveMoneda: string;
    public inicioPeriodoValidez: string;
    public finPeriodoValidez: string;
    public posiciones: Array<ContratoMarcoPosicion> = new Array<ContratoMarcoPosicion>();
}

export class ContratoMarcoPosicion {
    public numeroDocumentoCompras: string;
    public numeroPosicionDocumentoCompras: string;
    public indicadorDeBorrado: string;
    public numeroMaterial: string;
    public textoMaterialOServicio: string;
    public centro: string;
    public almacen: string;
    public cantidadPrevista: number;
    public unidadMedida: string;
    public importeMonedaBapi: number;
    public tipoPosicionDocumentoCompras: string;
    public tipoImputacionCompras: string;
    public numeroPaquete: string;
    public grupoArticuloMateriales: string;
    public subPosiciones: Array<ContratoMarcoSubposicion> = new Array<ContratoMarcoSubposicion>();
}

export class ContratoMarcoSubposicion {
    public numeroDocumentoCompras: string;
    public numeroPosicionDocumentoCompras: string;
    public subposicion: string;
    public numeroServicio: string;
    public textoBreve: string;
    public cantidadPositivoONegativo: number;
    public unidadMedidaBase: string;
    public precioUnitario: number;
    public precioTotal: number;
    public grupoArticulo: string;
}

export class ObtenerContratoMarco {
    public centro: string;
    public numeroContrato: string;
}