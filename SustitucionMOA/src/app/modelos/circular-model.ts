export interface CircularDto{    
    Observacion: string
    UsuarioIds: number[]
    PlazoDeOferta: Date
    FechaEntrega: Date
    Adjuntos: Array<File>    
    RequiereCambioDeFecha: boolean
    PeticionDeOferta_Id: number
}



