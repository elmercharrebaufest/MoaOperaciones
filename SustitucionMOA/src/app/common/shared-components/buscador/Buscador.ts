export class Resultado {
    Link: string;
    Value: string;
    Tipo: String;
    Code: string;
    CtaParams: number;
    SubOpciones: SubOption[];
}
export class SubOption
{
    Nombre: string;
    Value: string;
}
export class ResultadoTipo {
    DetalleContrato: string = "DCNT";
    HistorialPesificacion: string = "HPES"
    Liquidacion: string = "LIQ"
    ProformaFinal: string = "PROF"
    ProformaFinalAgrupador: string = "PROFA"
    CartaPorte: string = "CCPP"
}