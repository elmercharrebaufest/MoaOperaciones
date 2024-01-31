import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../../services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { Resultado } from './Buscador';

@Injectable({
    providedIn: 'root'
})
export class BuscadorService extends BaseService {

    public palabraABuscar: string = "";

    public getResultados(palabraABuscar: string) {
        let params: HttpParams = new HttpParams();
        params = params.set('palabraABuscar', palabraABuscar);

        return this.http
            .get<Resultado[]>(`/api/Home/BuscardorInteligente`, { params: params, headers: this.headers });
    }

    descargarDocumentoPDF(documento: string, ejercicio: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('documento', documento);
        params = params.append('ejercicio', ejercicio);

        return this.http
            .get<any>('/api/PDF/downloadDocumentPDF', { params: params, headers: this.headers })
    }

    descargarProformaFinal(fijacion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('fijacion', fijacion);

        return this.http
            .get('/api/liquidacion/descargarProformaFinal', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    public descargarFotosCCPP(cartaPorteIds: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cartaPorteIds', cartaPorteIds);
       
        return this.http
            .get('/api/cartaporte/DescargarFotos', { params: params, headers: this.headers });
    }

    public limpiarBuscador() {
        this.palabraABuscar = "";
    }

}