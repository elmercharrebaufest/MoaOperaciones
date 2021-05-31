import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { Solp } from './Solp';

@Injectable()
export class ComprasService extends BaseService {

    public getCombos(): Observable<any> {
        return this.http
            .get('/api/compras/Combos', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public GuardarSolp(solp: Solp) {
        let solpJson = JSON.stringify({
            Id: solp.id,
            NombreDeObra: solp.nombreDeObra,
            FiscalContrato: solp.fiscalContrato,
            Telefono: solp.telefono,
            Email: solp.mail,
            FechaHoraEntrega: this.getFechaHora(solp.fechaEntrega, solp.horaEntrega),
            SupervisorSector: solp.supervisorSector,
            SupervisorTrabajo: solp.supervisorTrabajo,
            VisitasObraMasiva: solp.listaVisitas.map(x=> { return {Codigo: x.id, FechaHora: this.getFechaHora(x.visitaDeObraFecha, x.visitaDeObraHora) }}),
            TieneVisitaObra: solp.visitaDeObra,
            TieneVisitaObraMasiva: solp.visitaDeObraMasiva,
            TieneObradores: solp.obradores,
            TieneMedioElevacion: solp.modoElevacion,
            TieneTecnicoSeguridad: solp.tecnicoSeguridad,
            TieneDescripcionTecnica: solp.descripcionTecnica,
            TieneDocumentacionTecnica: solp.entregaDocumentacion,
            FechaHoraLimiteConsulta: this.getFechaHora(solp.fechaLimiteFecha, solp.fechaLimiteHora),
            ObservacionesGeneracion: solp.observacionesGeneracion,
            EspecificacionesTecnicas: solp.especificacionesViewModel.observaciones,
            DiasEjecucion: solp.ejecucion,
            ObservacionesCotizacion: solp.observacionesCotizacion,
            JornadaLaboral: solp.jornadaLaboralDias.filter(x=>x.selected).map(x=>x.weekDay),
            JornadaLaboralDesde: solp.comienzoJornadaLaboral,
            JornadaLaboralHasta: solp.terminoJornadaLaboral,
            Adjuntos: solp.especificacionesViewModel.archivosGuardadosEspecificaciones.map(x=>x.id)
        });
        var payload = new FormData();

        var archivos = solp.especificacionesViewModel.archivosAdjuntosNuevos;
        if(archivos != null)
        {
            for (let i = 0; i < archivos.length; i++) {
                let fileToUpload = archivos[i];
                payload.append("file", fileToUpload, fileToUpload.name);
            }
        }

        payload.append('solpJson', solpJson);
        
        return this.http
            .post('/api/compras/GuardarSolp',  payload , this.headers).pipe(
                map(this.extractData));
    }

    getFechaHora(fecha: Date, hora: Date){
        return new Date(fecha.getFullYear(), fecha.getMonth(), fecha.getDay(), hora.getHours(), hora.getMinutes(), hora.getSeconds(), 0);
    }
}