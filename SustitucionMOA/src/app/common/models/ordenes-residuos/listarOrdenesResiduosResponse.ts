export interface ListarOrdenesResiduosResponse {
    ListaOrdenes: OrdenResiduosFila[];
}

export interface OrdenResiduosFila {
    Id: number;
    RazonSocialCliente: string;
    DescripcionEstado: string;
    FechaCreacion: string;
    LocalidadDescripcion: string;
    Material: string;
    PatenteChasis: string;
    ColorSemaforo: string;
}