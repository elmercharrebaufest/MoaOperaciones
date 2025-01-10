import { TablaSap } from "../Common/tablaSap";

export interface MaterialSolp {
    Id: number;
    Codigo: string;
    CodigoSap: string;
    Descripcion: string;
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