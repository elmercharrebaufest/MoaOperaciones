import { TablaGeneral } from "../../Common/tablaGeneral";
import { TablaSap } from "../../Common/tablaSap";
import { MaterialSolp } from "../materialSolp";
import { SolpSubposicionPrecargada } from "./solpSubposicionPrecargada";

export interface SolpPosicionPrecargada {
    Indice: number;
    TipoPosicionId: number;
    TipoPosicion: TablaGeneral;
    TipoImputacionId: number;
    TipoImputacion?: TablaGeneral;
    CentroCodigo: number;
    Centro: TablaSap;
    Codigo: string;
    MaterialCatalogado?: MaterialSolp;
    Tarea: string;
    MonedaId: number;
    Moneda: TablaSap;
    FechaEntregaServicio: string;
    GrupoComprasId: number;
    GrupoCompras: TablaSap;
    GrupoArticuloId: number;
    GrupoArticulo: TablaSap;
    AlmacenId: number;
    Almacen: TablaSap;
    Cantidad: number;
    UnidadId: number;
    Unidad: TablaSap;
    CuentaMayor: TablaSap;
    Imputacion: TablaSap;
    Subposiciones: SolpSubposicionPrecargada[];
}