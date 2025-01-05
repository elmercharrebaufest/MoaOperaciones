export interface TablaGeneral {
    Id: number;
    Tabla: string;
    Codigo: string;
    Descripcion: string;
    IdPadre: number;
    Padre: TablaGeneral;
    CodigoVisualizacion: string;
}