
import { PosicionCompra } from "../compras/solp-compra"
import { SolpPosicion } from "../compras/solp/solp-posicion"
import { CotizacionDto } from "./cotizacionDto"

export interface PeticionDeOfertaDto{
    RespetaMateriales?: boolean
    RespetaServicios?: boolean
    Id?: number
    FechaEntregaFormateado?: string
    PlazoDeOferta?: Date,
    CUIT?: string
    Mail?: string
    Usuarios?: PeticionDeOfertaUsarioDto[],
    Solp_Id?: number,
    NroSolp?: string,
    FechaCreacion?: Date,
    UsuarioCreador_Id?: number,
    Observaciones?: string,
    TipoPosicionCodigo?: string,
    CotizacionId?: number,
    PeticionDeOfertaPosicion?: PeticionDeOfertaSolpPosicionDto[],
    Cotizacion?: CotizacionDto,
    ObservacionTecnica?: string,
    ObservacionEconomica?: string
    SolpDto?: any,
    Selected?: boolean
    Cantidad?: number
}

export interface PeticionDeOfertaUsarioDto {
    Id: number,
    RazonSocial: string,
    UsuarioId: number,
    Mail?: string,
    CUIT?: string,
    Cotizacion?: CotizacionDto,
    PropuestaTecnicaAprobada?: boolean,
    RealizoVisita?: boolean

}
    
export interface PeticionDeOfertaSolpPosicionDto{
    valorTotal: number
    Id: number
    PeticionDeOferta_Id?: number
    SolpPosicion_Id?: number
    Posicion?: SolpPosicion,
    Posiciones: PosicionCompra,
    SolpId: number    
}


