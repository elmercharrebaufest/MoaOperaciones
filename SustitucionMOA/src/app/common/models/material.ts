import { Almacen } from "./almacen";

export class Material {
    public Id: number;
    public Descripcion: string;
    public CampaniaActual: string;
    public CampaniaIdActual: number;
    public CodigoSap: string;
    public MaterialId: number;
    public ValidaSisaRuca: boolean;
    public Abreviacion: string;

    public Almacenes: Almacen[];

    constructor() { }
}

export const CODIGO_ACEITE_SOJA_NEUTRALIZADO = "98855";
export const CODIGO_ACEITE_METILADO_SOJA = "99098";
export const CODIGO_PELLET_GIRASOL_INTEGRAL = "99709";

export const CANTIDAD_DEFAULT = 30000;
export const CANTIDAD_PELLET_GIRASOL = 20000;

export const RETIRO_EN_PATAGONIA = [CODIGO_ACEITE_METILADO_SOJA, CODIGO_ACEITE_SOJA_NEUTRALIZADO];