import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { Observable, throwError } from 'rxjs';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import { map, timeoutWith } from 'rxjs/operators';
import { CartaPresentacion } from '../../common/models/cartaPresentacion';
import { InformeComercial } from '../../common/models/informeComercial';
import { BaseService } from './../../common/services/BaseService';

@Injectable()
export class EmpresaGranosService extends BaseService {

    postFile(files: FileList, fileKey: string, proveedorId: number): Observable<any> {
        let formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            let fileToUpload = files.item(i);
            formData.append('file', fileToUpload, fileToUpload.name);
        }

        formData.append('fileKey', fileKey);
        formData.append('proveedorId', proveedorId.toString());

        return this.http.post('/api/AltaEmpresaGranos/GuardarArchivo', formData).pipe(map(this.extractData));
    }

    searchLocalidad(term): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('localidad', term);

        return this.http.get('/api/AltaEmpresaGranos/GetLocalidadCombo', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    generarInformeComercial(informeComercial: InformeComercial): Observable<any> {
        let payload = new FormData();
        payload.append("informeComercialJson", JSON.stringify(informeComercial));
        return this.http
            .post('/api/AltaEmpresaGranos/GenerarInformeComercial', payload)
            .pipe(timeoutWith(30000, throwError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }


    generarCartaPresentacion(cartaPresentacion: CartaPresentacion, proveedorId?: number): Observable<any> {
        let payload = new FormData();
        payload.append("cartaPresentacionJson", JSON.stringify(cartaPresentacion));
        payload.append('proveedorId', proveedorId.toString());

        return this.http
            .post('/api/AltaEmpresaGranos/GenerarCartaPresentacion', payload)
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

    obtenerArchivosSubidos(mail?: string, proveedorId?: number): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        let params: URLSearchParams = new URLSearchParams();
        params.set('mail', mail);
        params.set('proveedorId', proveedorId.toString());
        return this.http
            .get('/api/AltaEmpresaGranos/ObtenerArchivosSubidos', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    obtenerInfoProveedor(proveedorId: number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('mail', "");
        params.set('proveedorId', proveedorId.toString());

        return this.http
            .get('/api/AltaEmpresaGranos/ObtenerInfoProveedor', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }


    descargarArchivoSubido(fileKey: string, archivoID?: number, proveedorId?: number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('mail', "");
        params.set('archivoID', archivoID.toString());
        params.set('proveedorId', proveedorId.toString());

        return this.http
            .get('/api/AltaEmpresaGranos/DescargarArchivo', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }


    enviarSolicitud(proveedorId: number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('proveedorId', proveedorId.toString());

        return this.http
            .get('/api/AltaEmpresaGranos/EnviarSolicitudUsuario', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

    eliminarArchivoSubido(archivoID?: number, proveedorId?: number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('archivoID', archivoID.toString());
        params.set('proveedorId', proveedorId.toString());

        return this.http
            .get('/api/AltaEmpresaGranos/EliminarArchivo', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }

}