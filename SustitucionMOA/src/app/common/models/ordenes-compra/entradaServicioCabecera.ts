import { EntradaServicioDetalle } from "./entradaServicioDetalle";

export class EntradaServicioCabeceraDto {
    ID: number;
    EntradaServicio: string;
    FechaCreacionDateTime?: Date;
    FechaCreacion: string;
    OrdenCompra: string;
    Proveedor: string;
    Descripcion: string;
    MontoTotal: string;
    CUIT: string;
    entradaServicioDetalle: EntradaServicioDetalle[];
    Fiscal: string;
    Area: string;
    Suplente: string;
    Aprobador: string;
    MotivoRechazo: string;
    Estado: string;
    NumeroCertificacion: string;
    Ingresante: string;
    DesdeSap: boolean;
    FechaAprobacion: string;
    FechaRechazo: string;
    Moneda: string;
    FechaContabilizacion: string;
    FechaDocumento: string;
    NroPosicion: string;
    AnuladaPor: string;
}