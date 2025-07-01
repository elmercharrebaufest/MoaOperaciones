export interface Certificacion {
    NombreDeArchivo: string;
    NRO_OC: string;
    NRO_Certificacion: string;
    Importe: number;
    Moneda: string;
    MontoFormateado: string;
    Archivo: ArchivoAdjunto[];
    EsMonedaExtranjera: boolean;
    Seleccionada: boolean;
}

export interface GrupoCertificaciones {
    nombreArchivo: string;
    items: Certificacion[];
    esMonedaExtranjera: boolean;
    esFacturaPorDiferenciaTasaDeCambio: boolean;
}

export interface ArchivoAdjunto {
    Id: string;
    Ruta: string;
}