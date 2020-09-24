
import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { BaseService } from './../../common/services/BaseService';



@Injectable()
export class EstadoSolicitudService extends BaseService {

    public getEstadoAprobacion(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresa/getEstadoAprobacion', { headers: this.headers })
            .pipe(
                map(this.extractData)
            );
    }

}