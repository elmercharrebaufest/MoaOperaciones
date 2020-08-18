import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { Observable, throwError } from 'rxjs';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import { debounceTime, map, timeoutWith } from 'rxjs/operators';
import { InformeComercial } from '../../common/models/informeComercial';
import { BaseService } from './../../common/services/BaseService';

@Injectable()
export class EmpresaGranosService extends BaseService {

    postFile(files: FileList, fileKey: string): Observable<any>  {
        let fileToUpload = files.item(0);
        let formData = new FormData();
        formData.append('file', fileToUpload, fileToUpload.name);
        formData.append('fileKey', fileKey);

        return this.http.post('/api/AltaEmpresaGranos/GuardarArchivo', formData).pipe(map(this.extractData));
    }

    searchLocalidad(term) {
        var listadoLocalidades = this.http.get('/api/AltaEmpresaGranos/GetLocalidadCombo' + term)
            .pipe(
                debounceTime(500),  // WAIT FOR 500 MILISECONDS ATER EACH KEY STROKE.
                map(
                    (data: any) => {
                        return (
                            data.length != 0 ? data as any[] : [{ "Localidad": "Sin resultados" } as any]
                        );
                    }
                ));

        return listadoLocalidades;
    }  

    generarInformeComercial(informeComercial : InformeComercial): Observable<any>  {
        let payload = new FormData();
        payload.append("informeComercialJson", JSON.stringify(informeComercial));
        return this.http
            .post('/api/AltaEmpresaGranos/GenerarInformeComercial', payload)
            .pipe(timeoutWith(30000, throwError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    obtenerMateriales(): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        return this.http
            .get('/api/AltaEmpresaGranos/GetMateriales', { headers: this.headers })
            .pipe(map(this.extractData));
    }

    obtenerArchivosSubidos(mail?: string): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        let params: URLSearchParams = new URLSearchParams();
        params.set('mail', mail);
        return this.http
            .get('/api/AltaEmpresaGranos/ObtenerArchivosSubidos', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    descargarArchivoSubido(fileKey: string, mail?: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fileKey', fileKey);
        params.set('mail', mail);

        return this.http
            .get('/api/AltaEmpresaGranos/DescargarArchivo', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }


    enviarSolicitud(): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        return this.http
            .get('/api/AltaEmpresaGranos/EnviarSolicitudUsuario', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }



}