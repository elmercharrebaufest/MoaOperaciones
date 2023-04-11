import { SolpPosicion } from "../compras/solp/solp-posicion"
import { CircularDto } from "./circular-model"
import { PeticionDeOfertaDto, PeticionDeOfertaUsarioDto } from "./peticion-de-oferta-model"

export interface OfertasProveedor {
    Id: number,
    Solp_Id: number,
    UsuarioCreador_Id: number,
    FechaCreacion: Date,
    PlazoDeOferta: Date,
    Observaciones: string,
    Usuarios: PeticionDeOfertaUsarioDto[],
    FechaEntrega: Date,
    CircularDto: CircularDto,
    SolpDto: PeticionDeOfertaDto,
    NroSolp: string,
    NombreDeObra: string,
    UsuarioCreador: string,
    TieneVisitaObra: string,
    TieneVisitaObraMasiva: boolean,
    PlazoDeOfertaCircular: Date,
    PeticionDeOfertaPosicion: SolpPosicion
}

