export interface PeticionDeOfertaDto{
    Id: number
    FechaEntregaFormateado: string
    PlazoDeOferta: Date
    Usuarios: PeticionDeOfertaUsarioDto[]
}

export interface PeticionDeOfertaUsarioDto{
    RazonSocial: string
    UsuarioId: number
}

