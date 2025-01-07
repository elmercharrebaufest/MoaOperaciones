import { TablaSap } from "../../Common/tablaSap";

export interface SolpSubposicionPrecargada {
    Numero: number;
    Tarea: string;
    Codigo: string;
    Cantidad: number;
    UnidadId: number;
    CuentaMayor: TablaSap;
    Unidad: TablaSap;
}