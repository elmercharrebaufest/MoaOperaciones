export interface CircularDto{    
    Observacion: string
    UsuarioIds: number[]
    PlazoDeOfertaFecha: Date
    PlazoDeOfertaHora: Date
    FechaEntrega: Date
    Adjuntos: Array<File>    
    RequiereCambioDeFecha: boolean
    PeticionDeOferta_Id: number
}



