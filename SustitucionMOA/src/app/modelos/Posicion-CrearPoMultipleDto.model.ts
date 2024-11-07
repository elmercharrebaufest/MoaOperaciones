import { SubPosicionCrearPoMultipleDto } from "./SubPosicion-CrearPoMultipleDto.model";

export interface PosicionCrearPoMultipleDto {
    Id: number;
    NroPosicion?: number;
    Descripcion: string;
    TipoImputacion: string;
    Centro: string;
    Almacen: string;
    Moneda: string;
    ValorTotal?: number;

    SubPosiciones: SubPosicionCrearPoMultipleDto[];

    Expanded: boolean;
}