import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { BaseService } from './../../common/services/BaseService';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';


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

}