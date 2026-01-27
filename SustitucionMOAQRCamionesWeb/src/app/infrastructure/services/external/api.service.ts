import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, of, tap, catchError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TrackingData } from '../../../models/tracking-data.model';
import { TrackingResponse } from '../../../models/tracking-response.model';
import { EstadoEtapas } from '../../../models/estado-etapas.model';
import { EstadoEtapasResponse } from '../../../models/estado-etapas-response.model';
import { LogRequestDto } from '../../../models/log-request.model';
import { FileItem, FilesResponse } from '../../../models/files.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  private apiKey = environment.apiKey;

  trackingData = signal<TrackingData | null>(null);
  estadoEtapasData = signal<EstadoEtapas | null>(null);
  filesData = signal<FileItem[]>([]);

  // ==================== Private Helpers ====================

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      'x-api-key': this.apiKey
    });
  }

  // ==================== Tracking ====================

  getTrackingData(ctg: string, patente: string, captcha?: string): Observable<TrackingResponse> {
    let params = new HttpParams()
      .set('request.ctg', ctg)
      .set('request.patente', patente)
      .set('request.tipoWorkflow', 'Ingreso por Compra de Granos - Calada Externa');

    if (captcha) {
      params = params. set('request.captcha', captcha);
    }

    return this.http
      .get<TrackingResponse>(`${this.apiUrl}/api/qrcamiones/search`, { 
        params, 
        headers: this.getHeaders() 
      })
      .pipe(
        tap((response) => {
          if (response.resultado && response.data) {
            this.trackingData.set(response. data);
          }
        }),
        catchError((error) => {
          console. error('Error fetching tracking data:', error);
          return of({
            resultado: false,
            mensaje: error.error?. mensaje || error.message || 'Datos no encontrados',
            data: null as any
          });
        })
      );
  }

  updateFromEstadoEtapas(estadoEtapas: EstadoEtapas): void {
    const current = this.trackingData();
    if (current) {
      this.trackingData.set({
        ...current,
        etapas: estadoEtapas.etapas,
        datosAdicionales: estadoEtapas.datosAdicionales
      });
    }
  }

  clearTrackingData(): void {
    this.trackingData.set(null);
  }

  updateCurrentStage(stageIndex: number): void {
    const current = this.trackingData();
    if (current) {
      this.trackingData.set({
        ...current,
      });
    }
  }

  // ==================== Estado Etapas ====================

  getEstadoEtapas(ctg: string, patente: string): Observable<EstadoEtapasResponse> {
    const params = new HttpParams()
      .set('request.ctg', ctg)
      .set('request.patente', patente)
      .set('request.tipoWorkflow', 'Ingreso por Compra de Granos - Calada Externa');

    return this.http
      .get<EstadoEtapasResponse>(`${this.apiUrl}/api/qrcamiones/estadoEtapas`, { 
        params, 
        headers: this.getHeaders() 
      })
      .pipe(
        catchError((error) => {
          console.error('Error fetching estado etapas:', error);
          return of({
            resultado: false,
            mensaje: error.error?.mensaje || error.message || 'Error al obtener datos'
          });
        })
      );
  }

  updateEstadoEtapas(data: EstadoEtapas): void {
    this.estadoEtapasData.set(data);
  }

  clearEstadoEtapas(): void {
    this.estadoEtapasData.set(null);
  }

  // ==================== Files ====================

  getFiles(ctg: string, patente: string): Observable<FilesResponse> {
    const params = new HttpParams()
      .set('request.ctg', ctg)
      .set('request.patente', patente);

    return this.http
      .get<FilesResponse>(`${this.apiUrl}/api/qrcamiones/files`, { 
        params, 
        headers: this.getHeaders() 
      })
      .pipe(
        tap((response) => {
          const rawFiles = response.data || [];

          if (rawFiles.length > 0) {
            const mappedFiles = rawFiles.map((file: any) => ({
              nombre: file.nombre,
              url: file.url,
              contenido: file. datos || file.contenido
            }));

            this.filesData.set(mappedFiles);
          } else {
            console.warn('API returned no files in data property');
          }
        }),
        catchError((error) => {
          console.error('Error fetching files:', error);
          return of({
            resultado: false,
            mensaje: error.error?.mensaje || error.message || 'Error al obtener archivos',
            data: []
          });
        })
      );
  }

  findFileByName(partialName: string): FileItem | undefined {
    const files = this.filesData();
    return files.find(file =>
      file.nombre.toLowerCase().includes(partialName.toLowerCase())
    );
  }

  downloadFile(file: FileItem): void {
    if (!file. url && !file.contenido) {
      console.error('No URL or content available for download');
      return;
    }

    if (file.url) {
      window.open(file.url, '_blank');
    } else if (file.contenido) {
      this.downloadFromBase64(file.contenido, file. nombre);
    }
  }

  private downloadFromBase64(base64Content: string, fileName:  string): void {
    try {
      const base64Data = base64Content.includes(',')
        ? base64Content.split(',')[1]
        : base64Content;

      const byteCharacters = atob(base64Data);
      const byteNumbers = new Array(byteCharacters.length);

      for (let i = 0; i < byteCharacters. length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
      }

      const byteArray = new Uint8Array(byteNumbers);
      const blob = new Blob([byteArray], { type: this.getMimeType(fileName) });

      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();

      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error downloading file:', error);
    }
  }

  private getMimeType(fileName: string): string {
    const extension = fileName.split('.').pop()?.toLowerCase();

    const mimeTypes: Record<string, string> = {
      'pdf': 'application/pdf',
      'jpg': 'image/jpeg',
      'jpeg': 'image/jpeg',
      'png': 'image/png',
      'zip': 'application/zip',
      'doc': 'application/msword',
      'docx': 'application/vnd.openxmlformats-officedocument. wordprocessingml.document'
    };

    return mimeTypes[extension || ''] || 'application/octet-stream';
  }

  clearFiles(): void {
    this.filesData. set([]);
  }

  // ==================== Logging ====================

  logToBackend(log: string, isError:  boolean): Observable<void> {
    const request: LogRequestDto = {
      log:  log,
      isError: isError
    };

    return this.http.post<void>(
      `${this.apiUrl}/api/qrcamiones/log`,
      request,
      { headers: this.getHeaders() }
    );
  }

  // ==================== Other ====================

  actualizarSolp(nroSolp: string): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/ActualizarSolpa? nrosolp=${nroSolp}`,
      {},
      { headers: this.getHeaders() }
    );
  }
}