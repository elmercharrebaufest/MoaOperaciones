export interface Certificacion {
    NombreDeArchivo: string;
    NRO_OC: string;
    NRO_Certificacion: string;
    Importe: number;
    Moneda: string;
    Archivo: ArchivoAdjunto[];
    Seleccionada: boolean;
}

export interface GrupoCertificaciones {
    nombreArchivo: string;
    items: Certificacion[];
}

export interface ArchivoAdjunto {
    Id: string;
    Ruta: string;
}