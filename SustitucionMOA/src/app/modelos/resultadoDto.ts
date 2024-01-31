import { AltaNuevoProveedor } from "../compras/solp-compra"

export interface ResultadoDto {
    Descripcion: string,
    HayError: Boolean,
    Errores: ErrorMensaje[],
    ProveedorDto: AltaNuevoProveedor
}

export interface ErrorMensaje {
    ErrorCode: number,
    Message: string
}