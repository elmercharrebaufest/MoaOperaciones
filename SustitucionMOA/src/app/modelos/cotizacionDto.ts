import { Archivo } from "../common/models/archivo"
import { LegajoDto } from "./legajoDto"
import { PeticionDeOfertaDto } from "./peticion-de-oferta-model"

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
    ArchivosCotizacion: Archivo[]
    TieneObservacionTecnica: boolean, 
    CotizacionPosiciones: CotizacionPosicionDto[],
    TieneAdjuntos: boolean    
    CotizacionesHoras: CotizacionHoraDto[]
}

export interface CotizacionPosicionDto {
    PlazoDeEntrega: number
    Id: number,
    Cotizacion_Id: number,
    PeticionDeOfertaSolpPosicion_Id: number, 
    Cantidad: number,
    UnidadDeMedida_Id: any, 
    Moneda_Id: any,
    Precio: number,
    PrecioTotal: number,
    FechaDeEntrega: any,
    FechaOriginal: any,
    Codigo: string,
    Descripcion: string,
    TextoSuministro: string,
    CantidadSolp: number,
    UnidadMedida: any,
    MonedaCodigo: string,
    Moneda: any;
    UnidadComprasDescripcion: string,
    CotizacionSubPosiciones?: CotizacionSubPosicionDto[]
    NoDisponible: boolean

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

export interface GuardarCotizacion{
    UnidadDeMedidaSubpos?: any
    CantidadSubpos?: number
    Posicion: string,
    PeticionDeOfertaSolpPosicionId: number
    Precio: number
    MonedaId: number
    UnidadDeMedidaId: number
    Cantidad: number
    FechaDeEntrega: Date    
    NoDisponible: boolean
}

export interface CotizacionHoraDto{
    Cotizacion_Id? : number
    Categoria?: string
    CantidadPersonas?: number
    HorasNormales?: number
    HorasNocturnas?: number
    HorasExtras?: number
    Gremio?:string,
    Fila?:boolean
    ConfigurarHora?:boolean
}

