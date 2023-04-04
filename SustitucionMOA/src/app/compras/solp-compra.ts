export interface SolpCompraDto{
    Id: number,
    NroSolp: string,   
    PosicionCompras: PosicionCompra[],
    TipoPosicionCodigo: string,
}

export interface PosicionCompra{
    Id: number,
    Indice: string,
    Tarea: string,
    MaterialComprasCodigo: number,
    CentroComprasDescripcion: string,
    AlmacenComprasDescripcion: string,
    TextoSuministro: string,
    Modelo: string,
    GrupoComprasDescripcion: string,
    Cantidad: number,
    UnidadComprasDescripcion: string,
    MonedaComprasDescripcion: string,
    FechaEntregaServicio: Date,
    FechaOferta: Date,
    PlazoEntrega: Date,
    ProveedoresCompras?: SolpProveedorDto[]
    SubposicionesCompras?: SolpSubposicionDto[]
    TieneCotizacion: boolean,
    Selected: boolean,
}

export interface SolpSubposicionDto{
    Numero: number,
    Tarea: string,
    Codigo: string,
    Cantidad: number,
    UnidadComprasDescripcion: string

}

export interface SolpProveedorDto{
    SolpPosicionId?: number,
    TipoFiltroProveedorSolpCodigo?: string
    RazonSocial?: string

}

export interface EnvioSolpCompra{
    SolpId: number,
    PosIds: number[],
    Observacion: string,
    UsuarioIds: number[],
    Adjuntos: Array<File>,
    Id?: number
}

export interface AltaNuevoProveedor{
    Id?: number,
    CUIT?: number,
    Mail?: string,
    RazonSocial?: string
}