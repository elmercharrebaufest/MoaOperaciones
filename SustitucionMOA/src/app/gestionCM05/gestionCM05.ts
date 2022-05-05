export interface CabeceraCM05 {
    FechaUltimaModificacionString: string;
    FechaCargaString: string;
    Id;
    Estado;
    EstadoId;
    CUIT;
    Anticipo;
    Sede;
    FechaCarga;
    FechaUltimaModificacion;
    MalCargada;
    Secuencia;
    SecuenciaId;
    ConsultaId;
    RazonSocial;
};

export interface DetalleCM05 {
    Id;
    Jurisdiccion;
    NumeroJurisdiccion;
    FechaInicio;
    FechaCese;
    CoeficienteIngresos;
    CoeficienteGastos;
    CoeficienteUnificado;
    FechaUltimaModificacion;
    Editar?;
};

export interface MovimientoCM05 {
    Id;
    Observaciones;
    Fecha;
    Tipo;
    TipoId;
    Origen;
    OrigenId;
    EstadoAnterior;
    EstadoAnteriorId;
    EstadoPosterior;
    EstadoPosteriorId;
};