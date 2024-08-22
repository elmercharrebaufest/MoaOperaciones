import { Injectable } from '@angular/core';
import { BaseService } from '../common/services/BaseService';
import { HttpClient } from '@angular/common/http';
import { Observable, ObservedValueOf } from 'rxjs';
import { ApiResponse } from '../common/models/response';
import { ActualizarProgresoReqDto, AsignarAlumnosResDto, AsignarReqDto, CursoDto, CursoUsuarioDto, ProgresoAlumnoEnCursoDto, ProgresoResDto } from '../common/models/cursos/Curso';

@Injectable()
export class CursosService extends BaseService {
  private baseUrl = "/api/Curso";
  constructor(http: HttpClient) {
    super(http)
  }
  asignadosAUsuario(): Observable<ApiResponse<CursoUsuarioDto[]>> {
    return this.http.get<ApiResponse<CursoUsuarioDto[]>>
      (`${this.baseUrl}/AsignadosAUsuario`)
  }
  disponibles(): Observable<ApiResponse<CursoDto[]>> {
    return this.http.get<ApiResponse<CursoDto[]>>
      (`${this.baseUrl}/Disponibles`)
  }
  asignar(req: AsignarReqDto): Observable<ApiResponse<AsignarAlumnosResDto[]>> {
    return this.http.post
      (`${this.baseUrl}/Asignar`, req)
  }
  progreso(cursoId: number): Observable<ApiResponse<ProgresoResDto>> {
    return this.http.get(`${this.baseUrl}/Progreso?cursoId=${cursoId}`)
  }

  actualizarProgreso(progreso: ActualizarProgresoReqDto): Observable<ApiResponse<boolean>> {
    return this.http.patch
      (`${this.baseUrl}/ActualizarProgreso`, progreso)

  }
  obtenerProgresoAlumnos(cursoId: number): Observable<ApiResponse<ProgresoAlumnoEnCursoDto[]>> {
    return this.http.get(`${this.baseUrl}/ObtenerProgresoAlumnos?cursoId=${cursoId}`)
  }
  obtenerProgresoAlumno(cursoId: number): Observable<ApiResponse<ProgresoAlumnoEnCursoDto>> {
    return this.http.get(`${this.baseUrl}/ObtenerProgresoAlumno?cursoId=${cursoId}`)
  }
}