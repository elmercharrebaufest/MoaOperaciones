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