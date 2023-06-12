export interface AdjudicacionDto{  
    Id?: number  
    Cotizacion_Id?: number
    AdjudicacionPosiciones?: AdjudicacionPosicionDto[]
    Solp_Id?: number
    Moneda_Id?: number
}

export interface AdjudicacionPosicionDto{
    Id?: number 
    Adjudicacion_Id?: number 
    CotizacionPosicion_Id?: number 
    Cantidad?: number 
    SolpPosicion_Id?: number 
}




