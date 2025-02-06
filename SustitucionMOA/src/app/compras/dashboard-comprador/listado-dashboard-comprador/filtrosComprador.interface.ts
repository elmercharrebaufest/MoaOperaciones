import { SelectItem } from "primeng/api";

export interface FiltrosComprador {
    nroSolp: string;
    nombrePedido: string;
    sap: boolean;
    mantenimiento: boolean;
    web: boolean;
    repoAutomatica: boolean;
    listarPendiente: SelectItem;
    contratoMarco: boolean;
    usuarios: string[];
    estadoSolp: string[];
    gruposCompras: string[];
    centros: string[];
    claseDocumento: string[];
    tipoImputacion: string[];
    valorTipoImputacion: string[];
    subtipoImputacionCombo: SelectItem[];
    fechaDesde: string;
    fechaHasta: string;
    pageIndex: number;
    selectTipoPliego: string[];
}