
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseService } from './../../common/services/BaseService';



@Injectable()
export class EstadoSolicitudService extends BaseService {

    public getEstadoAprobacion(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresa/getEstadoAprobacion', { headers: this.headers });
    }

}