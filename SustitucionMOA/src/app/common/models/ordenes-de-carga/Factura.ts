export interface Factura {
    NumeroFactura: string;
    NumeroPedido: string;
    KgDisponibles?: number;
    Label: string;
}
export function newFactura(factura: Factura): Factura {
    return { ...factura, Label: `${factura.NumeroFactura} - ${factura.KgDisponibles}` };
}