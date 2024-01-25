export interface InfoVisitasDeObraDto{
    CantidadVisitas: number, 
    DetalleVisitas : DetalleVisitaDeObraDto[]
}

export interface DetalleVisitaDeObraDto{
    FechaHora: Date,
    Proveedor: ProveedorVisitaDeObraDto,
    NroSolp: number
}

export interface ProveedorVisitaDeObraDto{
    RazonSocial: string, 
    Cuit: number
}

export interface VisitaObraDto
{
    Codigo: string, 
    FechaHora: Date
}



