export interface ExistenciaEmpresaResponse {
    Mensaje: string;
    Error: string;
    EmpresasExistentes: EmpresaExistenciaVerificada[];
}

export interface EmpresaExistenciaVerificada {
    Id: number;
    Codigo: string;
    Mail: string;
}