export class Pesada {
    numeroPesada: number;
    fecha: string;
    hora: string;
    pesoTara: number;
    pesoBruto: number;
    pesoNeto: number;
}

export class CargaPesadas {
    pesadas: Array<Pesada>;
    balanza: string;
    fecha: string;
    hora: string;
    bodega: string;
    commodity: string;
    destino: string;
    exportador: string;
    vapor: string;
    pesoProgramado: number;
    pesoAcumulado: number;
}