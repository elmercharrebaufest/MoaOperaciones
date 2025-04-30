import { PosicionCrearPoMultipleDto } from "./Posicion-CrearPoMultipleDto.model";

export interface SolpCrearPoMultipleDto {
    Id: number;
    NroSolp: string;
    Nombre: string;
    FechaCreacion: Date;
    FechaLiberacion: Date;
    Solicitante: string;
    GrupoDeCompras: string;
    Centro: string;
    Tipo: string;

    MultipleFinalizado:boolean;

    Posiciones: PosicionCrearPoMultipleDto[];

    Selected: boolean;
    Expanded: boolean;
    SeraUsadoEnPliegoMultiple: boolean;
}