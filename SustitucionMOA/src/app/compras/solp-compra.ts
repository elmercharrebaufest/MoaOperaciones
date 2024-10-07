import { CotizacionPosicionDto } from "../modelos/cotizacionDto"
import { RegistroInfoDto } from "../modelos/registro-info"

export interface SolpCompraDto {
    Id: number,
    NroSolp: string,
    PosicionCompras: PosicionCompra[],
    TipoPosicionCodigo: string,
    RegistrosInfo?: RegistroInfoDto[],
    PlazoDeOfertaTentativo?: Date,
    MostrarSelectorPlazoDeOferta?: boolean,
}

export interface PosicionCompra{
    NroSolp?: string
    TipoPosicionCodigo: string
    PrecioTotal: number
    UnidadId: string
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
    FechaEntregaServicio: any,
    FechaOferta: Date,
    PlazoEntrega: Date,
    ProveedoresCompras?: SolpProveedorDto[]
    SubposicionesCompras?: SolpSubposicionDto[]
    TieneCotizacion: boolean,
    Selected: boolean,
     //Cotizacion
    CotizacionPosicion?: CotizacionPosicionDto
    NoDisponible: boolean,
    TodasPosicionesSeleccionadas: boolean

}

export interface SolpSubposicionDto{
    CotizacionSubPosicionId: any
    UnidadId: any
    Numero?: number,
    Tarea?: string,
    Codigo?: string,
    Cantidad?: number,
    UnidadComprasDescripcion?: string
    Id?: number,    
    CantidadCotizacion?: number
    UnidadCotizacionDescripcion: string
    UnidadCotizacionId: number
    MonedaCotizacionDescripcion: string,
    MonedaCotizacionCodigo: string
    MonedaCotizacionId: number
    PrecioSubPosicion: number
    PrecioTotalSubPosicion: number


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
    Id?: number,
    AdjuntoPliego: boolean,
    PlazoDeEntrega: Date,
}

export interface AltaNuevoProveedor{
    Id?: number,
    CUIT?: number,
    Mail?: string,
    RazonSocial?: string,
    EsProveedorExterior?: boolean
}