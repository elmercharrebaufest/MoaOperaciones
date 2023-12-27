import { PosicionCompra } from "../compras/solp-compra"
import { SolpPosicion } from "../compras/solp/solp-posicion"
import { CotizacionDto } from "./cotizacionDto"

export interface PeticionDeOfertaDto {
    PorcentajeDeHoras?: any
    PersonalHoras?: boolean,
    RespetaMateriales?: boolean,
    RespetaServicios?: boolean,
    Id?: number,
    FechaEntregaFormateado?: string,
    PlazoDeOferta?: Date,
    PlazoDeOfertaHora?: Date
    CUIT?: string,
    Mail?: string,
    Usuarios?: PeticionDeOfertaUsarioDto[],
    UsuariosAdicionales?: PeticionDeOfertaUsarioAdicionalDto[],
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
    PlazoDeOfertaEstado?: string,
    Adicional?: boolean,
    NroOrdenDeCompraAdicional?: string,
    TieneVisitaObraMasiva?: boolean,
    TieneVisitaObraBool?: boolean,
    Estado?: string,
    RevisionFinalizada?: boolean,
    RevisionTecnica?: PeticionDeOfertaRevisionTecnicaDto
    PideDescripcionTecnica?: boolean,
    PideDocumentacionTecnica?: boolean
}

export interface PeticionDeOfertaUsarioDto {
    CodigoProveedor: string
    Id: number,
    RazonSocial: string,
    UsuarioId: number,
    Mail?: string,
    CUIT?: string,
    Cotizacion?: CotizacionDto,
    PropuestaTecnicaAprobada?: boolean,
    RealizoVisita?: boolean,
    EstaHabilitado: boolean,
    ValidacionCircularSolicitante?: boolean,
    ObservacionNoCumple?: string,
    Deshabilitado?: boolean,
    Centro: any
}

export interface PeticionDeOfertaUsarioAdicionalDto {
    Id: number,
    RazonSocial: string,
    UsuarioId: number,
    Mail?: string,
    CUIT?: string   
}

export interface PeticionDeOfertaSolpPosicionDto {
    CantidadPendiente: number
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
    CentroId?: number
}

export interface PeticionDeOfertaCierreDto {
    Id: number
    PeticionDeOferta_Id?: number
    Usuario_Id?: number
    Fecha?: Date,
    Observaciones: string
}

export interface PeticionDeOfertaRevisionTecnicaDto{
    Id: number,
    Usuario_Id?: number,
    RecotizacionEconomica?: boolean,
    ModificacionSolp?: boolean,
    ObservacionRecotizacion?: string,
    Finalizada?: boolean
}
