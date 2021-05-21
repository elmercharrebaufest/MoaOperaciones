export interface Consulta {
    Id;
    CodigoCorredor;
    RazonSocialCorredor;
    CodigoProveedor;
    RazonSocialProveedor;
    CategoriaId;
    SubCategoriaId?;
    Asunto;
    EstadoConsultaId;
    FechaCreacion;
    FechaUltimaModificacion;
    UsuarioId;
    UsuarioActualId;

    Fecha;
    ComprobanteNo;
    OtroComprobanteNo;
    ContratoNo;
    Importe;
    Impuesto;
    BolsaEmisoraOblea;

    Categoria;
    SubCategoria?;
    EstadoConsulta;
    CausaConsulta;
    Comentarios;
    
    DiasReclamo;
    Usuario?;
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

export class Comentario{
    consulta_Id: any; 
    Detalle: any; 
    Fecha: any
}
