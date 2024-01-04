export interface AdjudicacionDto{
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
    Id?: number 
    Adjudicacion_Id?: number 
    CotizacionPosicion_Id?: number 
    Cantidad?: number 
    SolpPosicion_Id?: number 
}




