export interface ListarOrdenesResiduosResponse {
    ListaOrdenes: OrdenResiduosFila[];
}

export interface OrdenResiduosFila {
    Id: number;
    Corredor: string;
    Cliente: string;
    Estado: string;
}