import { TipoPerfil } from "../enums/TipoPerfil";

export interface SeleccionarVendedorResponse {
    CodigoVendedor: string;
    Descripcion: string;
    Noticias: any
    EsCodigoCorredor: boolean
    TipoUsuario: TipoPerfil
    ProveedorId: number
}