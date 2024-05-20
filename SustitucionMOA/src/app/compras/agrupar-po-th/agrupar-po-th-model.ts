export interface UsuarioDto {
    Mail: string;
}

export interface TablaGeneralDto {
    Descripcion: string;
    Codigo: string;
}

export interface TablaSapDto {
    Id: number;
    CodigoSap: string;
    Descripcion: string;
}

export interface SolpSubposicionDto {
    Numero: number;
    Tarea: string;
    Codigo: string;
    Cantidad: number;
    UnidadComprasDescripcion: string;
}

export interface SolpPosicionDto {
    Id: number;
    Codigo: string;
    Indice: number;
    Tarea: string;
    CentroComprasDescripcion: string;
    CentroCodigoSap: string;
    AlmacenComprasDescripcion: string;
    TextoSuministro: string;
    Modelo: string;
    GrupoComprasDescripcion: string;
    GrupoComprasCodigoSap: string;
    MaterialComprasCodigo: string;
    MaterialDescripcion: string;
    Cantidad: number;
    UnidadComprasDescripcion: string;
    MonedaSolpDescripcion: string;
    FechaEntregaServicio: Date;
    FechaOferta: Date | null;
    PlazoEntrega: number;
    SubposicionesCompras: SolpSubposicionDto[];
}

export interface SolpDto {
    UsuarioActual: UsuarioDto;
    Id: number;
    NroSolp: string;
    NombreDeObra: string;
    FechaCreacionFormateada: string;
    FechaCreacion: Date;
    TipoSolp: TablaGeneralDto;
    TipoSolpSap: string;
    Adicional: boolean;
    NroOrdenDeCompraAdicional: string;
    TrabajoYaHecho: boolean;
    ProveedorAsignadoCuit: string;
    ProveedorAsignadoRazonSocial: string;
    ItemPorPagina: number;
    Pagina: number;
    EstadoSolpSap: TablaSapDto;
    NroPeticionDeOferta: number;
    Agrupada: boolean;
    PosicionCompras: SolpPosicionDto[];
}
