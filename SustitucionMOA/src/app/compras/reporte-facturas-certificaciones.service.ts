import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { BaseService } from '../common/services/BaseService';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReporteFacturasCertificacionesService extends BaseService {

  public GetDatosReporte(
    periodo: string,
    fechaInicio: string,
    fechaFin: string,
    pagina?: number,
    itemsPorPagina?: number,
    orden?: string,
    columna?: string,
    ordenCompra?: string,
    proveedor?: string
  ): Observable<any> {
    let params: HttpParams = new HttpParams();

    // Solo agregar parámetros de fecha si están presentes
    if (periodo) {
      params = params.append('periodo', periodo);
    }
    if (fechaInicio) {
      params = params.append('fechaInicio', fechaInicio);
    }
    if (fechaFin) {
      params = params.append('fechaFin', fechaFin);
    }

    // Parámetros de paginación
    if (pagina !== undefined) {
      params = params.append('pagina', pagina.toString());
    }
    if (itemsPorPagina !== undefined) {
      params = params.append('itemsPorPagina', itemsPorPagina.toString());
    }
    if (orden) {
      params = params.append('orden', orden);
    }
    if (columna) {
      params = params.append('columna', columna);
    }

    // Parámetros de filtro específicos
    if (ordenCompra) {
      params = params.append('ordenDeCompra', ordenCompra);
    }
    if (proveedor) {
      params = params.append('proveedor', proveedor);
    }

    return this.http
      .get('/api/compras/ObtenerReporteFacturasCertificaciones', { params: params, headers: this.headers });
  }
}