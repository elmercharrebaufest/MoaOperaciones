
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { environment } from '../../environments/environment';

@Injectable()
export class ContactoMailService extends BaseService {


    public sendContactoMail(
        proveedor: string,
        nombre: string,
        email: string,
        telefono: string,
        categoria: string,
        camposAdicionales: string,
        comentario: string,
        contrato: string,
        razonSocial: string,
        cuit: string,
        nombreVendedor: string,
        comprobante: string,
        fechaPago: string,
        importe: string,
        impuesto: string,
        inscripcion: string,
        motivo: string,
        archivo: any
    ): Observable<any> {
        var payload = new FormData();
        var data = {
            proveedor: proveedor, nombre: nombre, email: email, telefono: telefono, categoria: categoria, camposAdicionales: camposAdicionales, comentario: comentario, contrato: contrato, razonSocial: razonSocial,
            cuit: cuit, nombreVendedor: nombreVendedor, comprobante: comprobante, fechaPago: fechaPago, importe: importe, importeDecimal: 0, impuesto: impuesto, inscripcion: inscripcion, motivo: motivo
        }
        payload.append("contacto", JSON.stringify(data));
        payload.append("file", archivo);
        return this.http
            .post('/api/contactoMail/sendContactoMail', payload, this.headersPost).pipe(
            map(this.extractData));
    }


    public getCategorias(): Observable<any> {
        return this.http
            .get('/api/contactoMail/getCategorias', { headers: this.headers }).pipe(
            map(this.extractData));
    }

}