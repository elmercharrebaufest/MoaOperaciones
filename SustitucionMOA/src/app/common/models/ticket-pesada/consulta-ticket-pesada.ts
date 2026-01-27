
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

export class ConsultaListadoTicketPesada {
    fechaInicio: Date = new Date();
    fechaEgreso: Date = new Date();
    patente: string = "";
    ctg: string = "";
    cuitIntermediarioFlete: string = "";
    cuitTransportista: string = "";
}