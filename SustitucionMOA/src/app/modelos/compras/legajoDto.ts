export interface LegajoDto {
    SolpId: number
    PeticionDeOfertaId: number
    Observacion: string
    FechaFormateado: string
    Tipo: string
    Leido: boolean
    ArchivoId?: number
    Usuario: any
}

export namespace LegajoTipo {
    export const ChatExterno = "Chat Externo"
    export const ChatInterno = "Chat Interno"
    export const CierreOferta = "Cierre de Oferta"
    export const Circular = "Circular"
    export const Cotizacion = "Cotización"
    export const Legajo = "Legajo"
    export const PeticionDeOferta = "Petición de Oferta"
    export const PeticionDeOfertaAgrupada = "Petición de Oferta Agrupada"
    export const PeticionDeOfertaVisualizacionPrecio = "Visualizacion de Precio"
    export const Pliego = "Pliego"
    export const RevisionTecnica = "Revisión tecnica"
    export const Solp = "SOLP"
    export const SolpArchivos = "SOLP Archivos"
    export const HistorialMovimientos = "HistorialMovimientos"
}
