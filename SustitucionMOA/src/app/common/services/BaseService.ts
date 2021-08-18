import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class BaseService {

    headers: HttpHeaders;
    headersPost: HttpHeaders;
    //options: any;

    constructor(protected http: HttpClient) {
        this.headers = new HttpHeaders();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        this.headersPost = new HttpHeaders();
        this.headersPost.append('Content-Type', 'application/json; charset=utf-8');
        this.headersPost.append('Cache-control', 'no-cache');
        this.headersPost.append('Cache-control', 'no-store');
        this.headersPost.append('Expires', '0');
        this.headersPost.append('Pragma', 'no-cache');
        //this.options = new RequestOptions({ headers: this.headers });
    }

    // protected extractData(res: any) {
    //     return res.json();
    // }

    getData(periodo?: string, fecha_inicio?: string, fecha_fin?: string, contrato?: string, pago?:string, retencion?:string): Observable<any>{
        return null;
    }

    exportExcel(periodo?: string, fecha_inicio?: string, fecha_fin?: string, contrato?: string, pago?: string, retencion?: string): Observable<any> {
        return null;
    }

    getDetalle(id: string): Observable<any> {
        return null;
    }

    handleError(error: any): Promise<any> {
        console.log(error)
        return Promise.reject(error.message || error);
    }
}