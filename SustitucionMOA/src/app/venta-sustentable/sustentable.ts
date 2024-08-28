export interface CampoProveedor {
    HectareasTotales: number;
    HectareasSoja: number;
    Longitud: string;
    Latitud: string;
    Proveedor_Id: number;
    CampoCosecha?: CampoCosecha;
    CampoCosecha_Id?: number;
    CUIT: string;
    Archivo_Id: number;
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