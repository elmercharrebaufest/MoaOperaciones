export interface DetallesCampo {
    HectareasTotales: number;
    HectareasSoja: number;
    Longitud: string;
    Latitud: string;
    Proveedor_Id: number;
    CUIT: string;
    Archivo_Id: number;
    EvidenciaEPA_Id: number;
    BSVS2: boolean;
    EPA: boolean;
    EUDR: boolean;
}
export interface CampoProveedor extends DetallesCampo {
    CampoCosecha?: CampoCosecha;
    CampoCosecha_Id?: number;
    RazonSocial?: string;
}
export interface CampoCosecha {
    Campo: CampoSustentable;
    Campo_Id?: number;
    Cosecha_Id: number;
    ToneladasAprobadas?: number;
}
export interface CampoSustentable {
    Nombre: string;
    Localidad_Id?: number;
    Renspa: string;
}
export interface CampoProveedorDetalle extends DetallesCampo {
    NombreCampo: string;
    Renspa: string;
    NombreCosecha: string;
    ToneladasAprobadas: number;
    CampoCosechaId: number;
    ProveedorNombre: string;
    LocalidadNombre: string;
    Localidad_Id: number;
    CampoSustentableId: number;
    CosechaId: number;
    CodigoProveedor: string;
    NombreArchivoEPA: string;
}

export interface SugerenciaCampo extends CampoProveedorDetalle {
    CampoYaPresentado: boolean;
    Seleccionado: boolean;
    NombreNuevoKmz: string;
    NombreNuevaEvidenciaEPA: string;
    Normativas: CampoCosechaNormativa[];
}

export interface CampoCosechaNormativa {
    Id: number;
    DescripcionNormativa: string;
    ToneladasAprobadas: number;
}