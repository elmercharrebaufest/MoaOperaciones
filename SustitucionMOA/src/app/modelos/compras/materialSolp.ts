import { TablaSap } from "../Common/tablaSap";
import { MaterialServicioSolp } from "./materialServicioSolp";

export interface MaterialSolp extends MaterialServicioSolp {
    // Id: number;
    // Codigo: string;
    CodigoSap: string;
    // Descripcion: string;
    TipoMaterial: string;
    TipoValoracion: string;
    PrecioMaterial: number | null;
    Estado: boolean;
    GrupoArticulo: TablaSap;
    CentroLogistico: TablaSap;
    UnidadMedidaBase: TablaSap;
    UnidadMedidaCompras: TablaSap;
    UnidadMedidaSalida: TablaSap;
    GrupoCompras: TablaSap;
    CuentaMayor: TablaSap;
}