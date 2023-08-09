export interface RegistroInfoDto{
    Numero?: number  
    Confirmado?: boolean
    PosicionId?: number  
    DescripcionPosicion?: string
    Precio?: number
    Cantidad?: number
    Centro?: string
    NombreProveedor?: string
    Moneda?: string     
    Fecha?: Date   
    Codigo?: string
    Unidad?: string 
    CantidadAdjudicacion?: number,
    Cuit?: string
    Indice?: string
    MaterialCodigo?: string
}

export interface MaterialAgrupado {
    MaterialCodigo: string;
    Cantidad?: number;
    CantidadAdjudicacionTotal: number;
    Indice?: string
}





