export class CotizacionHistorialDto {
  Id: number;
  Cotizacion_Id: number;
  Log: string;
  FechaFinalizacion: string;
  Usuario_Id: number;
  UsuarioRazonSocial: string;
  Cotizacion: LogCotizacionDto;
  MostrarHoras?: boolean;

}

export class LogCotizacionDto {
  Id: number;
  UsuarioCreador_Id: number;
  CotizacionEstadoDescripcion: string;
  PeticionDeOfertaUsuario_Id: number;
  RespetaMateriales?: string;
  RespetaServicios?: string;
  ObservacionTecnica: string;
  ObservacionEconomica: string;
  Revision: number;
  Archivos: ArchivoDto[] = [];
  FechaCreacion: string;
  PorcentajeDeHoras?: number;
  CotizacionPosiciones: LogCotizacionPosicionDto[];
  CotizacionesHoras: LogCotizacionHorasDto[];
}

export class LogCotizacionPosicionDto {
  Id: number;
  Cotizacion_Id: number;
  PeticionDeOfertaSolpPosicion_Id: number;
  Cantidad?: number;
  Precio?: number;
  FechaDeEntrega: string;
  MonedaCodigo: string;
  UnidadMedidaDescripcion: string;
  PrecioTotal: number;
  NoDisponible: string;
  FechaDeVigencia: string;
  PrimerPlazoDeOferta?: number;
  PrimeraCantidad?: number;
  SegundoPlazoDeOferta?: number;
  SegundaCantidad?: number;
  TercerPlazoDeOferta?: number;
  TerceraCantidad?: number;
  EstaEliminado: string;
  CotizacionSubPosiciones: LogCotizacionSubPosicionDto[];
  Indice?: number;
  IdPosicion: number;
  Descripcion: string;
  Codigo?: number;
  MostrarSubposiciones?: boolean;
  


}

export class LogCotizacionSubPosicionDto {
  CotizacionSubPosicionId?: number;
  Precio?: number;
  MonedaCodigo: string;
  UnidadDeMedidaDescripcion: string;
  Cantidad?: number;
  SolpSubPosicionId: number;
  PrecioTotal: number;
  NroSubPosicion: number;
  IdSubPosicion: number;
  Descripcion: string;
  Codigo?: number;
}

export class LogCotizacionHorasDto {
  Id: number;
  Cotizacion_Id: number;
  Categoria: string;
  CantidadPersonas: number;
  HorasNormales: number;
  HorasNocturnas: number;
  Gremio: string;
}

export interface ArchivoDto {
  Id: number,
  FileKey: string,
  Nombre: string,
  Ruta: string   
}