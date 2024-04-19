import { DatePipe } from "@angular/common";
import { Proveedor } from "../../common/models/proveedor";
import { PeticionDeOfertaDto } from '../../modelos/peticion-de-oferta-model';


export interface ChatsDto {
    ChatProveedores: ChatProveedorDto[]
    ChatCompras: ChatComprasDto
}

export interface ChatComprasDto {
    Mensajes?: ChatInternoComprasDto[], 
    Solp_Id?: number,
    FechaCreacion?: string,
    //Proveedores?: Proveedor[],
    UsuarioActualId?: number,
    FechaCreacionDate?: Date,
    RazonSocialComprador?: string,

}

export interface ChatInternoComprasDto {
    Id?: number, 
    Usuario_Id?: number,
    RolUsuario?: string,
    Solp_Id?: number,
    FechaEnvio?: string,
    Leido?: boolean,
    Mensaje?: string,
    Mail?: string,
    FechaDiaEnvio?: string,
    FechaEnvioDate?: Date
}

export interface ChatProveedorDto {
    Id?: number, 
    Usuario_Id?: number,
    RolUsuario?: string,
    PeticionDeOferta_Id: number,
    PeticionDeOfertaUsuario_Id: number,
    FechaEnvio?: string,
    Leido?: boolean,
    Mensajes?: ChatExternoComprasDto[], 
    Mail?: string,
    FechaDiaEnvio?: string,
    FechaEnvioDate?: Date
    CuitProveedor?: string
    RazonSocialProveedor?: string
}

export interface ChatExternoComprasDto {
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
    PeticionDeOfertaUsuario_Id: number
}