import { LegajoDto } from "./legajoDto";

export interface ObtenerLegajoResponse {
    LegajoFilas: LegajoDto[];
    PuedeVerPrecios: boolean;
}