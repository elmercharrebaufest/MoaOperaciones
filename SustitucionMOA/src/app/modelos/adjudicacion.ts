export interface AdjudicacionDto{
    PeticionDeOferta?: any
    EsMonedaProveedor?: boolean  
    Id?: number  
    Cotizacion_Id?: number
    AdjudicacionPosiciones?: AdjudicacionPosicionDto[]
    Solp_Id?: number
    Moneda_Id?: number
    TextoDeCabecera?: string
    CondicionesDeEntrega?: string
    CondicionesDePago?: string
    Garantias?: string      
    TipoPosicionCodigo ?: string
    NumeroOrdenDeCompra ?: string
    FechaCreacion ?: string
    Proveedor ?: string
    MonedaDescripcion?: string
    PrecioFinal ?: number 
    PrecioBruto?: number
    EstadoLiberacionCodigo?: string
    EstadoLiberacionDetalle?: string,
    RegionSap?: any 

}

export interface AdjudicacionPosicionDto{
    PrecioUnidad?: number
    Indice?: string
    FechaEntregaServicio?: any
    MonedaId?: any
    PlazoDeEntrega?: any
    Id?: number 
    Adjudicacion_Id?: number 
    CotizacionPosicion_Id?: number 
    Cantidad?: number 
    SolpPosicion_Id?: number 
}


export interface AdjudicacionEdicionDto {
    Solp_Id?: number;
    Id: number;
    TipoPosicionCodigo: string;
    NumeroOrdenDeCompra: string;
    Proveedor: string;
    Centro: string | undefined;
    CalleEntrega: string | undefined;
    CodigoPostal: string | undefined;
    PrecioFinal: number;
    TextoDeCabecera: string;
    CondicionesDeEntrega: string;
    CondicionesDePago: string;
    Garantias: string;
    Moneda_Id: number;
    MonedaDescripcion: string;
    FechaCreacion: Date;
    CondicionDePago: CondicionDePagoDto;
    CondicionDeImportacion: CondicionDeImportacionDto;
    AdjudicacionPosiciones: AdjudicacionPosicionEdicionDto[];
    PagoEn1: string | undefined;
    PagoEn2: string | undefined;
    PagoEn3: string | undefined;
    PagoEn1Porcentaje: string | undefined;
    PagoEn2Porcentaje: string | undefined;
}

export interface CondicionDePagoDto {
    Id: number | undefined;
    Codigo: string;
    CodigoDescripcion: string;
    Descripcion: string | undefined;
    
}

export interface CondicionDeImportacionDto {
    Id: number | undefined;
    Codigo: string;
    Descripcion: string | undefined;
    CodigoDescripcion: string;
}

export interface AdjudicacionPosicionEdicionDto {
    DireccionDeEntrega: OrdenDeCompraSAPPosicionDireccionDeEntrega;
    RegionCodigo: string;
    PaisSap: string;
    RegionId: number | undefined;
    SolpPosicion_Id: number;
    Id: number;
    MaterialComprasCodigo: string | undefined;
    MaterialComprasDescripcion: string | undefined;
    MaterialTextoAmpliado: string | undefined;
    Indice: number;
    Tarea: string;
    TextoSuministro: string;
    Modelo: string; // Puedes cambiar el tipo según tus necesidades
    Cantidad: number;
    PrecioUnidad: number;
    MonedaId: any | undefined;
    UnidadId: number | undefined;
    UnidadDescripcion: string;
    UnidadCodigo: string;
    MonedaDescripcion: string;
    MonedaCodigo: string | undefined;
    PrecioTotal: number;
    CentroComprasCodigo: string;
    Eliminado: boolean;
    EntregaFinal: boolean;
    Moneda: TablaSapDto;
    FechaEntregaServicio: any | null;
    FechaEntregaServicioFormateado: string;
    PlazoDeOferta: number;
    SubposicionesCompras?: SolpSubposicionDto[];
}

export interface OrdenDeCompraSAPPosicionDireccionDeEntrega {
    RegionSap: string;
    PaisSap: string;
    Id: number | undefined;
    CodigoSap: string;
    Descripcion: string;
}

export interface SolpSubposicionDto {
    Numero: number;
    Tarea: string;
    CodigoSolp: number;
    Cantidad: number;
    PrecioBruto: number;
    UnidadComprasDescripcion: string;
    MonedaCotizacionDescripcion: string;
    MonedaCotizacionCodigo: string | undefined;
    PrecioTotalSubPosicion: number;
    Eliminado: boolean;
}

export interface TablaSapDto {
    Codigo: string | undefined;
    Descripcion: string;
}






