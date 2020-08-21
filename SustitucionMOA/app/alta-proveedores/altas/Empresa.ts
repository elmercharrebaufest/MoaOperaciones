export class Empresa {       
    Id: number;
    CodigoProveedor: string;
    CUIT: string;
    RazonSocial: string;
    EstadoAprobacion: number;
    EstadoAprobacionDescripcion: string;
    IdComercialDataAgro: number;
    IdDataAgro: number;
    Mail: string;
    Observaciones: string;
    Comercial: string;
    HistorialAprobaciones: any;
}

export class HistorialAprobaciones {
    Id: number;
    Usurario: string;
    EstadoAprobacionDescripcion: string;
    Observaciones: string;
    Fecha: Date;
}
