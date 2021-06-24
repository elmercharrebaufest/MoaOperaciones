
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from './../common/services/BaseService';

@Injectable()
export class ApikeyService extends BaseService {


    public generarApikey(): Observable<any> {
        return this.http
            .get('/api/usuario/ObtenerNuevaApiKey', { headers: this.headers }).pipe(
            map(this.extractData));
    }
}