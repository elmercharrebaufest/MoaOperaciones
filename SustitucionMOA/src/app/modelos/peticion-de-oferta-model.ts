import { SolpPosicion } from "../compras/solp/solp-posicion"
export interface PeticionDeOfertaDto {
    Id: number
    FechaEntregaFormateado: string
    PlazoDeOferta: Date,
    CUIT: string
    Mail?: string
    Usuarios: PeticionDeOfertaUsarioDto[]
}

export interface PeticionDeOfertaUsarioDto {
    Id: number
    RazonSocial: string
    UsuarioId: number
    Mail?: string
    CUIT?: string
    Cotizacion?: CotizacionDto,
    PropuestaTecnicaAprobada?: boolean
    RealizoVisita?: boolean
}

export interface CotizacionDto {
    Id: number
    TieneObservacionTecnica: boolean
    ObservacionTecnica: string
    ObservacionEconomica: string
    FechaCreacion: Date
    RespetaMateriales?: boolean
    RespetaServicios?: boolean
    Revision: number
    CotizacionEstadoDescripcion: string
    Archivos: LegajoDto[]
}

export interface LegajoDto {
    Id: number
    Observacion: string
}
    
export interface PeticionDeOfertaSolpPosicionDto{
    Id: number
    PeticionDeOferta_Id: number
    SolpPosicion_Id: number
    Posicion: SolpPosicion
}