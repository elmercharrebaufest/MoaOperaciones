export interface ListarOrdenesResiduosResponse {
    ListaOrdenes: OrdenResiduosFila[];
}

export interface OrdenResiduosFila {
    Id: number;
    RazonSocialCorredor: string;
    RazonSocialCliente: string;
    DescripcionEstado: string;
    FechaCreacion: string;
    FechaRetiro: string;
    LocalidadDescripcion: string;
    Material: string;
    PatenteChasis: string;
    ColorSemaforo: string;
}