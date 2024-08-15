import { LegajoDto } from "./compras/legajoDto"

export interface LegajoExternoDto {
    ListaLegajos?: LegajoDto[]
    OrdenDeCompraNro?: string
    FechaAdjudicacionFormateado?: string
    Proveedor?: any
}