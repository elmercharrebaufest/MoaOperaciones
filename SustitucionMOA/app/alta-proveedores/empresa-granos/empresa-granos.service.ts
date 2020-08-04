import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { BaseService } from './../../common/services/BaseService';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Http, Response, URLSearchParams } from '@angular/http';


@Injectable()
export class EmpresaGranosService extends BaseService {


    postFile(files: FileList, fileKey: string) {

        let fileToUpload = files.item(0);
        let formData = new FormData();
        formData.append('file', fileToUpload, fileToUpload.name);
        formData.append('fileKey', fileKey);

        this.http.post('/api/AltaEmpresaGranos/GuardarArchivo', formData).subscribe((val) => {

            console.log(val);
        });
        //const endpoint = 'your-destination-url';
        //const formData: FormData = new FormData();
        //formData.append('fileKey', fileToUpload, fileToUpload.name);
        //return this.http
        //    .post('/api/AltaEmpresa/UploadFile', formData, { headers: this.headers })
        //    .pipe(map(() => { this.extractData; }))


    }


    generarInformeComercial(EmplRelDep: string, EmplRelDepCant: string, Rodados: string, RodadosOtros: string, Chacra: string, ChacraOtros: string, AntigActividad: string, ActuacionProd: string, ClienteAnt: string, Comentarios: string, Domicilio: string): Observable<any>  {
        let params: URLSearchParams = new URLSearchParams();
        params.set('EmplRelDep', EmplRelDep);
        params.set('EmplRelDepCant', EmplRelDepCant);
        params.set('Rodados', Rodados);
        params.set('RodadosOtros', RodadosOtros);
        params.set('Chacra', Chacra);
        params.set('ChacraOtros', ChacraOtros);
        params.set('AntigActividad', AntigActividad);
        params.set('ActuacionProd', ActuacionProd);
        params.set('clienteAnt', ClienteAnt);
        params.set('Comentarios', Comentarios);
        params.set('Domicilio', Domicilio);

        return this.http
            .get('/api/AltaEmpresaGranos/GenerarInformeComercial', { search: params, headers: this.headers })
            .pipe(map(this.extractData));

    }

    
}