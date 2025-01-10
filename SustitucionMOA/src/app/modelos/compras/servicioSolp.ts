import { MaterialServicioSolp } from "./materialServicioSolp";

export interface ServicioSolp extends MaterialServicioSolp {
    // Id: number;
    // Codigo: number;
    // Descripcion: string;
    GrupoArticulos: number | null;
    TipoServicio: string;
    AmbitoServicio: string;
    Edicion: number;
    UnidadMedidaBase: string;
    SSCItem: string;
}