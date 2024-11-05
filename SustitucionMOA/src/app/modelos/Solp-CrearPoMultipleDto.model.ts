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

    Selected: boolean;
}