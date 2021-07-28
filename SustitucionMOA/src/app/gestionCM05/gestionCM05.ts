export interface CabeceraCM05 {
    Id;
    CUIT;
    Anticipo;
    Sede;
    FechaCarga;
    FechaUltimaModificacion;
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