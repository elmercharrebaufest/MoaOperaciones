import { Solp } from "../compras/solp/solp";

export interface HistorialDeFechaDto {
    ListaSolp?: Solp[]; // Asumiendo que tienes una interfaz SolpDto ya definida en TS
    FechaCreacionPOFormateada?: string;
    Cuerpo?: string[][]; // Lista de listas de cadenas (2D array)
}
