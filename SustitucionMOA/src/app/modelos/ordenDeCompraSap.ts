export interface OrdenDeCompraSap{
    Cabecera: OrdenDeCompraSAPCabecera;
    Error: ErrorOC,
    //Posiciones:  OrdenDeCompraSAPPosicion();
}

export interface OrdenDeCompraSAPCabecera {
    OrdenDeCompra: string,  //nro orden de compra
    CodigoProveedor: string,  //codigo de proveedor, con otra rfc buscar el vendedor
    RazonSocialProveedor: string,
    Usuario_Id: number,
    Moneda: string,
    FechaCreacion: string,
    MontoTotal: number
}

// export interface OrdenDeCompraSAPPosicion{
//     Cabecera: OrdenDeCompraSAPCabecera;
//     Posiciones:  OrdenDeCompraSAPPosicion();
// }

export interface ErrorOC
    {
        Mensaje: string,
        Tipo: string,
    }
