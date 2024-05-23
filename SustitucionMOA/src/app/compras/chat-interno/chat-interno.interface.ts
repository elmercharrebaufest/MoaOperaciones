import { DatePipe } from "@angular/common";
import { Proveedor } from "../../common/models/proveedor";

export interface ChatComprasDto {
    Mensajes?: ChatInternoComprasDto[], 
    PeticionDeOferta_Id?: number,
    FechaCreacion?: string,
    Proveedores?: Proveedor[],
    UsuarioActualId?: number,
    FechaCreacionDate?: Date,

}

export interface ChatInternoComprasDto {
    Id?: number, 
    Usuario_Id?: number,
    RolUsuario?: string,
    PeticionDeOferta_Id?: number,
    FechaEnvio?: string,
    Leido?: boolean,
    Mensaje?: string,
    Mail?: string,
    FechaDiaEnvio?: string,
    FechaEnvioDate?: Date
}