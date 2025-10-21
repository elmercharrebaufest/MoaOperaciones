import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;
  private apiKey = environment.apiKey;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Accept': 'application/json',
      'x-api-key': this.apiKey
    });
  }

  actualizarSolp(nroSolp: string): Observable<any> {
    return this.http.post(
      `${this.apiUrl}ActualizarSolpa?nrosolp=${nroSolp}`,
      {},
      { headers: this.getHeaders() }
    );
  }

  logError(context: string, error: any): Observable<any> {
    return this.http.post(
      `${this.apiUrl}LogQr`,
      { context: context, error: error },
      { headers: this.getHeaders() }
    );
  }
}
