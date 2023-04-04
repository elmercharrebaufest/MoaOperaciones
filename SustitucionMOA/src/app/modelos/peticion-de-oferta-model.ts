export interface PeticionDeOfertaDto{
    Id: number
    FechaEntregaFormateado: string
    PlazoDeOferta: Date,
    CUIT: string
    Mail?: string
    Usuarios: PeticionDeOfertaUsarioDto[]
}

export interface PeticionDeOfertaUsarioDto{
    Id: number
    RazonSocial: string
    UsuarioId: number
    Mail?: string
    CUIT?: string
}

