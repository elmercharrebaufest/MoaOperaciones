
import { throwError as observableThrowError, Observable } from 'rxjs';

import { timeoutWith } from 'rxjs/operators';
import { Injectable } from '@angular/core';

import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
import { ApiResponse } from '../common/models/response';
import { ArchivoBoletoDto } from '../common/models/archivo-boleto/archivoBoletoDto';

@Injectable()
export class ArchivoBoletoService extends BaseService {

    getData(fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    listarArchivosBoleto(fechaInicio: string, fechaFin: string,): Observable<ApiResponse<ArchivoBoletoDto[]>> {
        let params = {
            fechaInicio,
            fechaFin
        }

        return this.http
            .get<ApiResponse<ArchivoBoletoDto[]>>('/api/ArchivoBoleto/ListarArchivosBoleto', { params: params, headers: this.headers }).pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    crearArchivoBoleto(archivo: File): Observable<ApiResponse<ArchivoBoletoDto>> {
        const data = new FormData();

        data.append("archivo", archivo)

        return this.http
            .post<ApiResponse<ArchivoBoletoDto>>('/api/ArchivoBoleto/CrearArchivoBoleto', data, { headers: this.headersPost }).pipe(
        );
    }

}