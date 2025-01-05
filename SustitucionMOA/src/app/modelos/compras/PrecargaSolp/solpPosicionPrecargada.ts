import { TablaGeneral } from "../../Common/tablaGeneral";
import { TablaSap } from "../../Common/tablaSap";
import { SolpSubposicionPrecargada } from "./solpSubposicionPrecargada";

export interface SolpPosicionPrecargada {
    Indice: number;
    TipoPosicionId: number;
    TipoPosicion: TablaGeneral;
    Codigo: string;
    TipoImputacionId: number;
    TipoImputacion?: TablaGeneral;
    Tarea: string;
    MonedaId: number;
    Moneda: TablaSap;
    FechaEntregaServicio: Date;
    GrupoComprasId: number;
    GrupoCompras: TablaSap;
    GrupoArticuloId: number;
    GrupoArticulo: TablaSap;
    CentroId: number;
    Centro: TablaSap;
    AlmacenId: number;
    Almacen: TablaSap;
    Cantidad: number;
    UnidadId: number;
    Unidad: TablaSap;
    CuentaMayor: TablaSap;
    Subposiciones: SolpSubposicionPrecargada[];
}