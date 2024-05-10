import { Injectable } from '@angular/core';
import { BaseService } from '../common/services/BaseService';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../common/models/response';
import { CursoDto, CursoUsuarioDto } from '../common/models/cursos/Curso';

@Injectable()
export class CursosService extends BaseService {
  private baseUrl = "/api/Cursos";
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
}