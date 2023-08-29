export interface DetalleReporteContrato {
    Acoplado: string;
    CantidadEntregada: number;
    CantidadEntregadaStr: string;
    CantidadFactura: number;
    CantidadFacturaStr: number;
    Chasis: string;
    Chofer: string;
    Destinatario: string;
    Entrega: string;
    Factura: string;
    FacturaLegal: string;
    FechaCarga: string;
    FechaPedido: string;
    NombreDestinatario: string;
    Pedido: string;
    Remito: string;
    CPE: string;
    OrdenCargaId: string;
}
export interface ReporteContrato {
    Contrato: string;
    PedidoCliente: string;
    PosNr: string;
    Cliente: string;
    NombreCliente: string;
    Corredor: string;
    DescripcionMaterial: string;
    KilosTotales: number;
    KilosTotalesStr: string;
    KilosEntregados: number;
    KilosEntregadosStr: string;
    KilosFacturados: number;
    KilosFacturadosStr: null | string;
    KilosPendienteEntrega: number;
    KilosPendienteEntregaStr: string;
    KilosPendienteFactura: number;
    FechaDesde: string;
    FechaHasta: Date;
    Precio: number;
    Moneda: string;
    Motivo: string;
    DetalleMotivo: string;
    CondicionEntrega: string;
    Producto: string;
    PuntoExpedicion: string;
    TipoContrato: string;
    ColorProducto: string;
    CodigoProducto: string;
    Detalles: DetalleReporteContrato[];
    NombreClienteCUIT: string;
}
export interface FiltrosReporteContrato {
    fechaInicio: string,
    fechaFin: string;
    mostrarPendientes: boolean
}