import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, Observable, of, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface FileItem {
  nombre: string;
  url?: string;
  contenido?: string;
}

export interface FilesResponse {
  resultado: boolean;
  mensaje?: string;
  data: FileItem[];
}

@Injectable({
  providedIn: 'root'
})
export class FilesService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  
  filesData = signal<FileItem[]>([]);

  getFiles(ctg: string, patente: string): Observable<any> {
    const params = new HttpParams()
      .set('request.ctg', ctg)
      .set('request.patente', patente);

    return this.http
      .get<any>(`${this.apiUrl}/api/qrcamiones/files`, { params })
      .pipe(
        tap((response) => {
          const rawFiles = response.data || [];

          if (rawFiles.length > 0) {
            const mappedFiles = rawFiles.map((file: any) => ({
              nombre: file.nombre,
              url: file.url,
              contenido: file.datos || file.contenido 
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
      file.nombre.toLowerCase(). includes(partialName.toLowerCase())
    );
  }

  downloadFile(file: FileItem): void {
    if (!file.url && !file.contenido) {
      console.error('No URL or content available for download');
      return;
    }

    if (file.url) {
      window.open(file.url, '_blank');
    } else if (file.contenido) {
      this.downloadFromBase64(file.contenido, file.nombre);
    }
  }

  private downloadFromBase64(base64Content: string, fileName: string): void {
    try {
      const base64Data = base64Content.includes(',') 
        ? base64Content.split(',')[1] 
        : base64Content;

      const byteCharacters = atob(base64Data);
      const byteNumbers = new Array(byteCharacters.length);
      
      for (let i = 0; i < byteCharacters.length; i++) {
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
    this.filesData.set([]);
  }
}