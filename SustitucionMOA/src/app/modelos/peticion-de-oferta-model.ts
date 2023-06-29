
import { PosicionCompra } from "../compras/solp-compra"
import { SolpPosicion } from "../compras/solp/solp-posicion"
import { CotizacionDto } from "./cotizacionDto"

export interface PeticionDeOfertaDto{
    PersonalHoras?: boolean,
    RespetaMateriales?: boolean,
    RespetaServicios?: boolean,
    Id?: number,
    FechaEntregaFormateado?: string,
    PlazoDeOferta?: Date,
    CUIT?: string,
    Mail?: string,
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
    ObservacionEconomica?: string,
    SolpDto?: any,
    Selected?: boolean,
    Cantidad?: number,
    PlazoDeOfertaEstado?: string
}

export interface PeticionDeOfertaUsarioDto {
    Id: number,
    RazonSocial: string,
    UsuarioId: number,
    Mail?: string,
    CUIT?: string,
    Cotizacion?: CotizacionDto,
    PropuestaTecnicaAprobada?: boolean,
    RealizoVisita?: boolean,
    EstaHabilitado: boolean,
    ValidacionCircularSolicitante?: boolean
}
    
export interface PeticionDeOfertaSolpPosicionDto{
    CantidadPendiente: any
    AdjudicacionCompleta: boolean
    valorTotal: number
    Id: number
    PeticionDeOferta_Id?: number
    SolpPosicion_Id?: number
    Posicion?: SolpPosicion,
    Posiciones: PosicionCompra,
    SolpId: number,
    expanded: boolean,
    Selected: boolean,
    TodasPosicionesSeleccionadas: boolean,
    NoDisponible: boolean,
}


