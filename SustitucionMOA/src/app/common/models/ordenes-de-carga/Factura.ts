import { Formatter } from "../../formatter/Formatter";

export interface Factura {
    NumeroFactura: string;
    NumeroPedido: string;
    KgDisponibles?: number;
    Label: string;
}
export function newFactura(factura: Factura): Factura {
    const { KgDisponibles, NumeroFactura } = factura;
    const kgLabel = KgDisponibles <= 0 ? 'sin kg Disp.' : !KgDisponibles ? '' :
        `${Formatter.formatNumberWithPoint(KgDisponibles.toString())} kg Disp.`;
    const Label = `${NumeroFactura}  ${kgLabel}`;
    return { ...factura, Label };
}