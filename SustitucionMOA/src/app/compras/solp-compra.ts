export interface SolpCompraDto{
    id: number,
    nroSolp: string,   
    posicionCompra: PosicionCompra[],
}

export interface PosicionCompra{
    indice: string,
    tarea: string,
    centroComprasDescripcion: string,
    almacenComprasDescripcion: string,
    textoSuministro: string,
    modelo: string,
    grupoComprasDescripcion: string,
    cantidad: number,
    unidadComprasDescripcion: string,
    monedaComprasDescripcion: string,
    fechaEntregaServicio: Date,
    plazoEntrega: Date,
    proveedoresCompras?: SolpProveedorDto[]
    subposicionesCompras?: SolpSubposicionDto[]
    tieneCotizacion: boolean,
}

export interface SolpSubposicionDto{
    numero: number,
    tarea: string,
    codigo: string,
    cantidad: number,
    unidadComprasDescripcion: string

}

export interface SolpProveedorDto{
    solpPosicionId?: number,
    tipoFiltroProveedorSolpCodigo?: string
    razonSocial?: string

}