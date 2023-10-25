import { LegajoDto } from "./legajoDto"

export interface LegajoExternoDto {
    ListaLegajos?: LegajoDto[]
    OrdenDeCompraNro?: string
    FechaAdjudicacionFormateado?: string
    Proveedor?: any
}