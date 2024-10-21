import { Archivo } from "../common/models/archivo"
import { PosicionCompra } from "../compras/solp-compra"
import { SolpPosicion } from "../compras/solp/solp-posicion"
import { CotizacionDto } from "./cotizacionDto"

export interface PeticionDeOfertaDto {
    Adicional?: boolean,
    ArchivosPaso4Cotizacion?: Archivo[],
    Cantidad?: number,
    Cotizacion?: CotizacionDto,
    CotizacionId?: number,
    CUIT?: string,
    EsNuevaCotizacion?: any,
    Estado?: string,
    FechaCreacion?: Date,
    FechaEntregaFormateado?: string,
    Id?: number,
    Mail?: string,
    NroOrdenDeCompraAdicional?: string,
    NroSolp?: string,
    NrosSolp?: string[],
    ObservacionCotizacion?: string,
    ObservacionEconomica?: string,
    ObservacionEconomicaOriginal?: any,
    ObservacionTecnica?: string,
    ObservacionTecnicaOriginal?: any,
    Observaciones?: string,
    PersonalHoras?: boolean,
    PeticionDeOfertaPosicion?: PeticionDeOfertaSolpPosicionDto[],
    PideDescripcionTecnica?: boolean,
    PideDocumentacionTecnica?: boolean,
    PlazoDeOferta?: Date,
    PlazoDeOfertaEstado?: string,
    PlazoDeOfertaHora?: Date,
    PorcentajeDeHoras?: any,
    RequisitoCiberseguridad?: boolean,
    RespetaMateriales?: boolean,
    RespetaServicios?: boolean,
    RevisionFinalizada?: boolean,
    RevisionTecnica?: PeticionDeOfertaRevisionTecnicaDto,
    Selected?: boolean,
    Solp?: any,
    SolpDto?: any,
    Solp_Id?: number,
    SolpModificada?: boolean,
    TieneVisitaObraBool?: boolean,
    TieneVisitaObraMasiva?: boolean,
    TipoPosicionCodigo?: string,
    UsuarioCreador_Id?: number,
    Usuarios?: PeticionDeOfertaUsarioDto[],
    UsuariosAdicionales?: PeticionDeOfertaUsarioAdicionalDto[],
    VerBotonVerPrecio: boolean
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
    EstaEliminado?: boolean
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

export interface PeticionDeOfertaRevisionTecnicaDto {
    Id: number,
    Usuario_Id?: number,
    RecotizacionEconomica?: boolean,
    ModificacionSolp?: boolean,
    ObservacionRecotizacion?: string,
    Finalizada?: boolean
}
