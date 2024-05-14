export interface CursoUsuarioDto {
    CursoId: number
    AccesoCurso: string;
    NombreCurso: string;
    EstadoCurso: EstadoCurso
}
export interface CursoDto {
    Id: number
    Nombre: string;
}

export enum EstadoCurso {
    SinIniciar = 0,
    Iniciado = 1,
    EnProgreso = 2,
    Completado = 3,
}

export interface AsignarReqDto {
    MailsUsuarios: string[];
    CursoId: number
}
export interface ActualizarProgresoReqDto {
    CursoId: number
    NuevoEstado: EstadoCurso
    EmailUsuario?: string;
    DatosProgreso: string
    TiempoSesion: string;
}
export interface AsignarAlumnosResDto {
    Resultado: boolean, Mail: string
}
export const CURSOS_BASE_PATH = "cursos"