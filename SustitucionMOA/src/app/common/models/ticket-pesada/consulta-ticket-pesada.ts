import { NumberSymbol } from "@angular/common"

export class ConsultaTicketPesada {
    NumeroCartaPorte: string = "";
    PatenteCamion: string = "";
    Mail: string = "";
}

export class ConsultaTicketPesadaSubproductos {
    PatenteCamion: string = "";
    FechaDesde: Date = new Date();
    FechaHasta: Date = new Date();
}