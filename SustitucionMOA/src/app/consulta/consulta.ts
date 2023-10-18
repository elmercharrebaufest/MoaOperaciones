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

export interface Materiales {
    MaterialId;
    Descripcion;
}

export class Comentario{
    consulta_Id: any; 
    Detalle: any; 
    Fecha: any;
    Recordado: any;
    FechaRecordado: any;
}
export class ReclamoImpositivo{
    Dni;
    RazonSocialEmpresa;
    RazonSocialProveedor;
    Cuit;
    Vinculo;
    Reclamos: Array<Reclamo>;
    Lugar;
}
export interface Reclamo{
    Fecha;
    Certificado;
    Importe;
}

export class Destinatario{
    Campo: string;
    Mail: string;
}