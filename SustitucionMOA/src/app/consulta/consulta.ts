import { DirOrden } from "../common/enums/DirOrden";
import { Archivo } from "../common/models/archivo";
import { Usuario } from "../usuario/usuario";

export interface Consulta {
    Id;
    CodigoCorredor;
    RazonSocialCorredor;
    CodigoProveedor;
    RazonSocialProveedor;
    CategoriaId;
    SubCategoriaId?;
    Asunto;
    Material;
    EstadoConsultaId;
    Material_Id;
    FechaCreacion;
    FechaUltimaModificacion;
    UsuarioId;
    UsuarioActualId;
    UsuarioInternoId;
    FechaVtoReapertura;


    Fecha;
    ComprobanteNo;
    OtroComprobanteNo;
    ContratoNo;
    Importe;
    Impuesto;
    BolsaEmisoraOblea;
    OrdenId;
    PatenteChasis;

    Categoria;
    SubCategoria?;
    EstadoConsulta;
    CausaConsulta;
    Comentarios: Comentario[];

    DiasReclamo;
    Usuario?;
    PuedeReabrir;

    RelacionadaPorCodigo?: boolean;
    GeneradaInternamente?: boolean;
    GeneradaExternamente?: boolean;
    GeneradaPorUsuarioSesion?: boolean;

    MailUsuarioIniciaConsulta?: string;
    Rubro: string;
}

export interface EstadoConsulta {
    Id: number;
    Code: string;
    Descripcion: string;
    Color?: string;
    Cantidad?: number;
}

export interface Categoria {
    Id;
    Code;
    Nombre;
    Cantidad;
}

export interface Subcategoria {
    Id;
    Code?;
    Nombre?;
    CategoriaId?;
    Cantidad?;
}

export interface Causa {
    Id;
    Nombre;
}

export interface Materiales {
    MaterialId;
    Descripcion;
}

export class Comentario {
    Consulta_Id?: any;
    consulta_Id: any;
    Detalle: string;
    Fecha: any;
    FechaRecordado: any;
    UsuarioId?: number;
    Id?: number;
    Usuario?: Usuario;
    Recordado?: boolean;
    CreadorInterno?: boolean;
    Archivos?: Archivo[];
    ComentarioRecordados?: any;
}
export class ReclamoImpositivo {
    Dni;
    RazonSocialEmpresa;
    RazonSocialProveedor;
    Cuit;
    Vinculo;
    Reclamos: Array<Reclamo>;
    Lugar;
}
export interface Reclamo {
    Fecha;
    Certificado;
    Importe;
}

export class Destinatario {
    Campo: string;
    Mail: string;
    UsuarioId: number;
    NombreTipoUsuario;
}

export enum OpcionFiltroAsociadaCreacion {
    Externa = "Externa",
    PorMOA = "Por MOA",
    PorUsuario = "Propias",
}

export function obtenerOpcionesFiltroPorCreacion(): Array<{ key: OpcionFiltroAsociadaCreacion, value: OpcionFiltroAsociadaCreacion }> {
    return [
        { key: OpcionFiltroAsociadaCreacion.Externa, value: OpcionFiltroAsociadaCreacion.Externa },
        { key: OpcionFiltroAsociadaCreacion.PorMOA, value: OpcionFiltroAsociadaCreacion.PorMOA },
        { key: OpcionFiltroAsociadaCreacion.PorUsuario, value: OpcionFiltroAsociadaCreacion.PorUsuario },
    ]
}
export interface ReqListadoConsultaDto {
    page: number;
    pageSize: number;
    orderBy: keyof Consulta;
    dirOrden: DirOrden;
    filtros?: Record<keyof Consulta, any>;
}