import { ArchivoDto } from "../../modelos/cotizacionDto";

  export interface AdjuntosSolpDto {
    Pliego: string;
    PDF: any;
    ArchivosEspecificacionesTecnicas: ArchivoDto[];
    ArchivosCotizacion: ArchivoDto[];
    ArchivosCondicionesEspeciales: ArchivoDto[];
    ObservacionCondicionesEspeciales: string;
  }