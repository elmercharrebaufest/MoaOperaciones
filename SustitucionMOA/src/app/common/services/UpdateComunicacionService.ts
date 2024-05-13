import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ComunicacionesService } from './../../comunicaciones/comunicaciones.service';


@Injectable({
    providedIn: 'root'
})
export class UpdateComunicacionService {

    constructor(protected comunicacionesService: ComunicacionesService) {
        this.comunicacionesService = comunicacionesService;
    }
    //Variable utilizada para suscripción desde ambos componentes
    public getAllCommunicationsObserv = new Subject<any>();

    getAllCommunicationsObserv$ = this.getAllCommunicationsObserv.asObservable();
    //Método a llamar desde ambos componentes
    public updateCommunications(idProveedor: string, start_date: string, end_date: string) {
            this.comunicacionesService.getComunicaciones(idProveedor, start_date, end_date).subscribe( res =>
            {
                this.getAllCommunicationsObserv.next(res);
            }
        );
    } 
}