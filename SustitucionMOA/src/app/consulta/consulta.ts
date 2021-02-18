export interface Consulta {
    Id;
    CodigoCorredor;
    RazonSocialCorredor;
    RazonSocialProveedor;

    Asunto;
    EstadoConsulta;
    EstadoConsultaId;
    FechaUltimaModificacion;
    Categoria;
    CategoriaId;
    IdCategoria;
    SubCategoria;
    FechaCreacion;
    DiasReclamo;

    ContratoNo;
    ComprobanteNo;

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
