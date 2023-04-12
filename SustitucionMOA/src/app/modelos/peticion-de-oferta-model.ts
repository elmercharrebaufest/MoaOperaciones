import { SolpPosicion } from "../compras/solp/solp-posicion"
import { CotizacionDto } from "./cotizacionDto"

export interface PeticionDeOfertaDto{
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
    
export interface PeticionDeOfertaSolpPosicionDto{
    Id: number
    PeticionDeOferta_Id: number
    SolpPosicion_Id: number
    Posicion: SolpPosicion
}


