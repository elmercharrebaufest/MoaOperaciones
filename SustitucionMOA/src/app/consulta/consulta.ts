export interface Consulta {
    Id;
    CodigoCorredor;
    RazonSocialCorredor;
    CodigoProveedor;
    RazonSocialProveedor;
    CategoriaId;
    SubCategoriaId;
    Asunto;
    EstadoConsultaId;
    FechaCreacion;
    FechaUltimaModificacion;
    UsuarioId;
    UsuarioActualId;

    Fecha;
    ComprobanteNo;
    ContratoNo;
    Importe;
    Impuesto;
    BolsaEmisoraOblea;

    Categoria;
    SubCategoria;
    EstadoConsulta;
    CausaConsulta;
    Comentarios;
    
    DiasReclamo;

}

export interface EstadoConsulta {
    Id;
    Descripcion;
    Color;
}

export interface Categoria {
    Id;
    Code;
    Nombre;
}

export interface Subcategoria {
    Id;
    Code;
    Nombre;
    CategoriaId;
}

export class Comentario{
    consulta_Id: any; 
    Detalle: any; 
    Fecha: any
}
