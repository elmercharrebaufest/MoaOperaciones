export interface EstadoCM05 {
    Id: number;
    Descripcion: string;
};

enum EstadoCabeceraCM05 {
    Pendiente = 0,
    Autorizado = 1,
    Completado = 2,
}

export interface CabeceraCM05 {
    Id;
    Estado;
    EstadoId;
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