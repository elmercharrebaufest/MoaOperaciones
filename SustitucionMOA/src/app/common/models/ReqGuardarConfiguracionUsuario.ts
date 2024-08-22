import { TipoConfiguracionUsuario } from "../enums/TipoConfiguracionUsuario";

export interface ReqGuardarConfiguracionUsuario {
    tipo: TipoConfiguracionUsuario,
    valor: string;
}