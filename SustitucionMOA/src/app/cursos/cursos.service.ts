import { Injectable } from '@angular/core';
import { BaseService } from '../common/services/BaseService';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../common/models/response';
import { AsignarReqDto, CursoDto, CursoUsuarioDto } from '../common/models/cursos/Curso';

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
  asignar(req: AsignarReqDto): Observable<ApiResponse<{ [key: string]: boolean }[]>> {
    return this.http.post
      (`${this.baseUrl}/Asignar`, req)
  }
}