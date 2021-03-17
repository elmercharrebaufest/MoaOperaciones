export interface CampoProveedor {
    HectareasTotales: number;
    HectareasSoja: number;
    Longitud: string;
    Latitud: string;
    Proveedor_Id: number;
    CampoCosecha: CampoCosecha
}
export interface CampoCosecha {
    Campo: CampoSustentable;
    Cosecha_Id: number;
}
export interface CampoSustentable{
    Nombre: string;
    Localidad_Id: number;
}