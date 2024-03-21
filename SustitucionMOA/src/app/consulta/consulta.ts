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
    Comentarios;

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
    Id;
    Code;
    Descripcion;
    Color;
    Cantidad;
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
    consulta_Id: any;
    Detalle: any;
    Fecha: any;
    Recordado: any;
    FechaRecordado: any;
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