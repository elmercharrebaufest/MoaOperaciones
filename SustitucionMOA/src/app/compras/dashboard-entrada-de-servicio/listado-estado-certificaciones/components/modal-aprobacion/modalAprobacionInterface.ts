interface certificacionES {
    Descripcion: string,
    Items: certificacionItemES[]
    MontoTotalACertificar: number,
    NumeroCertificacion: string,
    NroPosicion: string,
}

interface certificacionItemES {
    Cantidad: string,
    CantidadReal: number,
    CantidadACertificar: string,
    Descripcion: string,
    Importe: number,
    Moneda: string,
    MontoACertificar: number,
    NumeroLinea: number,
    Porcentaje: string,
    PorcentajeACertificar: number,
    ServicioNumero: number,
    UM: string
}