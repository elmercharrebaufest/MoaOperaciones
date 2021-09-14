export interface CabeceraCM05 {
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