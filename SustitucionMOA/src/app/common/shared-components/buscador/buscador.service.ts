import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../../services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { Resultado } from './Buscador';

@Injectable()
export class BuscadorService extends BaseService {

    public getResultados(palabraABuscar: string) {
        let params: HttpParams = new HttpParams();
        params = params.set('palabraABuscar', palabraABuscar);

        return this.http
            .get<Resultado[]>(`/api/Home/BuscardorInteligente`, { params: params, headers: this.headers });
    }
}