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

    constructor(entity: any =  null) {
        if (entity != null) {
            this.numeroDocumentoCompras = entity.NumeroDocumentoCompras;
            this.sociedad = entity.Sociedad;
            this.indicadorDeBorrado = entity.IndicadorDeBorrado;
            this.numeroCuentaProveedor = entity.NumeroCuentaProveedor;
            this.nombreProveedor = entity.NombreProveedor;
            this.organizacionCompras = entity.OrganizacionCompras;
            this.grupoCompras = entity.GrupoCompras;
            this.claveMoneda = entity.ClaveMoneda;
            this.inicioPeriodoValidez = entity.InicioPeriodoValidez;
            this.finPeriodoValidez = entity.FinPeriodoValidez;
            if (entity.Posiciones && entity.Posiciones.length) {
                this.posiciones = new Array<ContratoMarcoPosicion>();
                entity.Posiciones.forEach(pos => {
                    this.posiciones.push(new ContratoMarcoPosicion(pos));
                });
            }
        }
    }
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
    public selected: boolean;
    public allSubPosicionesSelected: boolean;

    constructor(entity: any = null) {
        if (entity != null) {
            this.numeroDocumentoCompras = entity.NumeroDocumentoCompras;
            this.numeroPosicionDocumentoCompras = entity.NumeroPosicionDocumentoCompras;
            this.indicadorDeBorrado = entity.IndicadorDeBorrado;
            this.numeroMaterial = entity.NumeroMaterial;
            this.textoMaterialOServicio = entity.TextoMaterialOServicio;
            this.centro = entity.Centro;
            this.almacen = entity.Almacen;
            this.cantidadPrevista = entity.CantidadPrevista;
            this.unidadMedida = entity.UnidadMedida;
            this.importeMonedaBapi = entity.ImporteMonedaBapi;
            this.tipoPosicionDocumentoCompras = entity.TipoPosicionDocumentoCompras;
            this.tipoImputacionCompras = entity.TipoImputacionCompras;
            this.numeroPaquete = entity.NumeroPaquete;
            this.grupoArticuloMateriales = entity.GrupoArticuloMateriales;            
            this.numeroDocumentoCompras = entity.NumeroDocumentoCompras;
            this.subPosiciones = new Array<ContratoMarcoSubposicion>();
            if (entity.SubPosiciones && entity.SubPosiciones.length) {
                entity.SubPosiciones.forEach(subpos => {
                    this.subPosiciones.push(new ContratoMarcoSubposicion(subpos));
                });
            }
        }
    }
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
    public selected: boolean;

    constructor(entity: any = null) {
        if (entity != null) {
            this.numeroDocumentoCompras = entity.NumeroDocumentoCompras;
            this.numeroPosicionDocumentoCompras = entity.NumeroPosicionDocumentoCompras;
            this.subposicion = entity.Subposicion;
            this.numeroServicio = entity.NumeroServicio;
            this.textoBreve = entity.TextoBreve;
            this.cantidadPositivoONegativo = entity.CantidadPositivoONegativo;
            this.unidadMedidaBase = entity.UnidadMedidaBase;
            this.precioUnitario = entity.PrecioUnitario;
            this.precioTotal = entity.PrecioTotal;
            this.grupoArticulo = entity.GrupoArticulo;
        }
    }
}

export class ObtenerContratoMarco {
    public centro: string;
    public numeroContrato: string;
}