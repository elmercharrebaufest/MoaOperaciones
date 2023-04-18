import { SolpCompraDto } from "../compras/solp-compra"
import { Solp } from "../compras/solp/solp"
import { SolpPosicion } from "../compras/solp/solp-posicion"
import { CotizacionDto } from "./cotizacionDto"

export interface PeticionDeOfertaDto{
    Id: number,
    FechaEntregaFormateado: string,
    PlazoDeOferta: Date,
    CUIT: string,
    Mail?: string,
    Usuarios: PeticionDeOfertaUsarioDto[],
    SolpDto: any,
    Selected: boolean
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
    Id: number,
    PeticionDeOferta_Id: number,
    SolpPosicion_Id: number,
    Posicion: SolpPosicion
}


