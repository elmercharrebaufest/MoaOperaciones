import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { LogRequestDto } from '../../models/log-request.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;
  private apiKey = environment.apiKey;

  private http = inject(HttpClient);

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Accept': 'application/json',
      'x-api-key': this.apiKey
    });
  }

  actualizarSolp(nroSolp: string): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}ActualizarSolpa? nrosolp=${nroSolp}`,
      {},
      { headers: this.getHeaders() }
    );
  }
  
  logToBackend(log: string, isError: boolean): Observable<void> {
    const request: LogRequestDto = {
      log: log,
      isError: isError
    };

    return this.http.post<void>(
      `${this.apiUrl}/api/qrcamiones/log`,
      request,
      { headers: this.getHeaders() }
    );
  }
}