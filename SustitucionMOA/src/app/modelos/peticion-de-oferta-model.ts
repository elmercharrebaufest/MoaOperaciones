
import { PosicionCompra } from "../compras/solp-compra"
import { SolpCompraDto } from "../compras/solp-compra"
import { Solp } from "../compras/solp/solp"
import { SolpPosicion } from "../compras/solp/solp-posicion"
import { CotizacionDto } from "./cotizacionDto"

export interface PeticionDeOfertaDto{
  
    Id?: number
    FechaEntregaFormateado?: string
    PlazoDeOferta?: Date,
    CUIT?: string
    Mail?: string
    Usuarios?: PeticionDeOfertaUsarioDto[],
    SolpDto?: any,
    Selected?: boolean
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
    ObservacionEconomica?: string,
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


