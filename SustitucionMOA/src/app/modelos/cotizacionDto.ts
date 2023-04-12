import { LegajoDto } from "./legajoDto"

export interface CotizacionDto {
    Id: number,
    UsuarioCreador_Id: number, 
    CotizacionEstado_Id: number,
    PeticionDeOfertaUsuario_Id: number,
    FechaCreacion: Date,
    RespetaMateriales: boolean, 
    RespetaServicios: boolean,
    ObservacionTecnica: string,
    ObservacionEconomica: string,
    Revision: number,
    CotizacionEstadoDescripcion: string, 
    Archivos: LegajoDto[],
    TieneObservacionTecnica: boolean, 
    CotizacionPosiciones: CotizacionPosicionDto[],
    TieneAdjuntos: boolean
}

export interface CotizacionPosicionDto {
    Id: number,
    Cotizacion_Id: number,
    PeticionDeOfertaSolpPosicion_Id: number, 
    Cantidad: number,
    UnidadDeMedida_Id: number, 
    Moneda_Id: number,
    Precio: number,
    FechaDeEntrega: string,
    Codigo: string,
    Descripcion: string,
    TextoSuministro: string,
    CantidadSolp: number,
    UnidadMedida: number,
    CotizacionSubPosiciones: CotizacionSubPosicionDto[]
}

export interface CotizacionSubPosicionDto {
    Id: number,
    CotizacionPosicion_Id: number,
    SolpSubPosicion_Id: number,
    Cantidad: number,
    UnidadDeMedida_Id: number,
    Moneda_Id: number,
    Precio: number
}