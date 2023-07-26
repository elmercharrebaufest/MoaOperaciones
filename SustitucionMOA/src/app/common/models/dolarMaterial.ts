export interface DolarMaterial {
    Desde: string;
    Hasta: string;
    FechaCotizacion: Date
    Cotizacion: number
    DesdeString: string
    HastaString: string
    FechaCotizacionString: string
}

export interface DolarGirasol extends DolarMaterial{}
export interface DolarMaiz extends DolarMaterial{}