import { TablaSap } from "../../Common/tablaSap";
import { ServicioSolp } from "../servicioSolp";

export interface SolpSubposicionPrecargada {
    Numero: number;
    Tarea: string;
    Codigo: string;
    Cantidad: number;
    Precio: number;
    UnidadId: number;
    CuentaMayor: TablaSap;
    Imputacion: TablaSap;
    Unidad: TablaSap;
    ServicioCatalogado: ServicioSolp;
}