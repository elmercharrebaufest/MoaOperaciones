export class Resultado {
    Link: string;
    Value: string;
    Tipo: String;
    Code: string;
    CtaParams: number;
}

export class ResultadoTipo {
    DetalleContrato: string = "DCNT";
    HistorialPesificacion: string = "HPES"
    Liquidacion: string = "LIQ"
    ProformaFinal: string = "PROF"
}