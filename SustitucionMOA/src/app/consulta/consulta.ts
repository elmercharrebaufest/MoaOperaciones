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
    id;
    nombre;
    color;
}

export interface Categoria {
    id;
    value;
    label;
    camposAdicionales;
}

