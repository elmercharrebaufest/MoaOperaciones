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
    SISAEstadoCuit: string;
    UltimaEdicion: Date;
    EstadoSIPER: string;
    IdTipoUsuario: number;
    AltaInterna: boolean;
    SiperObligatorio: boolean;

    FacturacionAnual?: number;
    ServicioPrestado?: string;
    OrganizacionDeCompra?: string; 
    RealizarAnalisisNOSIS?: boolean;
    RequiereVerificacionCompras?: boolean;
    IngresoAPlanta?: boolean;
    Rubro?: string;
    Telefono?: any;
    CondicionDePago?: any;
    RazonDeEleccion?: any;
    SolicitanteInterno?: any;
    ContieneDocumentacionFisica : boolean;
}

export class HistorialAprobaciones {
    Id: number;
    Usurario: string;
    EstadoAprobacionDescripcion: string;
    Observaciones: string;
    Fecha: Date;
}
