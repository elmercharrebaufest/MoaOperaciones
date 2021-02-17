export interface Consulta {
    id;
    asunto;
    estado;
    idEstado;
    fechaUltimaModificacion;
    categoria;
    idCategoria;
    fechaCreacion;

    contrato;
    razonSocial;
    cuit;
    comprobante;
    inscripcion;

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
