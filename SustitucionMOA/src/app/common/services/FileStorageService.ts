import { Injectable } from "@angular/core";

interface ArchivoBD {
    key: string;
    archivo: File;
}

@Injectable({
    providedIn: 'root',
})
export class FileStorageService {
    private db: IDBDatabase;
    private dbReady: Promise<void>;

    constructor() {
        this.dbReady = new Promise((resolve, reject) => {
            let openRequest = indexedDB.open('file-storage', 1);

            openRequest.onupgradeneeded = (event) => {
                console.info(`Conexión a BD file storage - UpgradeNeeded. OldVersion: ${event.oldVersion}, NewVersion: ${event.newVersion}`);
                this.db = openRequest.result;
                this.db.createObjectStore('archivosContacto', {keyPath: 'key'});
            }
            
            openRequest.onerror = (event) => {
                console.error("Error en BD file storage", openRequest.error);
                reject(openRequest.error);
            }

            openRequest.onsuccess = (event) => {
                this.db = openRequest.result;
                resolve();
            }
        })
    }

    guardarArchivoContacto(consultaId: number, archivo: File): Promise<void> {
        return this.dbReady.then(() => {
            return new Promise<void>((resolve, reject) => {
                let fileKey = `${consultaId}-${archivo.name}`;

                let transaction = this.db.transaction('archivosContacto', 'readwrite');
                let storeArchivos = transaction.objectStore('archivosContacto');

                let archivoBD: ArchivoBD = {key: fileKey, archivo: archivo};

                let request = storeArchivos.add(archivoBD);

                request.onsuccess = () => {
                    resolve();
                };
                request.onerror = () => {
                    console.error('Error al agregar al store el archivo', request.error);
                    reject(request.error);
                };
            });
        });
    }

    obtenerArchivosContacto(consultaId: number): Promise<ArchivoBD[]> {
        return this.dbReady.then(() => {
            return new Promise<ArchivoBD[]>((resolve, reject) => {
                let transaction = this.db.transaction('archivosContacto', 'readonly');
                let store = transaction.objectStore('archivosContacto');
                let archivosRequest = store.getAll();
                
                archivosRequest.onsuccess = () => {
                    let archivosBD = archivosRequest.result as ArchivoBD[];
                    archivosBD = archivosBD.filter(x => x.key.startsWith(consultaId.toString()));
                    resolve(archivosBD);
                }
                archivosRequest.onerror = () => {
                    console.error('Error al intentar obtener todos los archivos');
                    reject(archivosRequest.error);
                }
            });
        });
    }

    borrarArchivoContactoAutoguardado(consultaId: number, archivo: File): Promise<void> {
        return this.dbReady.then(() => {
            return new Promise<void>((resolve, reject) => {
                let fileKey = `${consultaId}-${archivo.name}`;
                let transaction = this.db.transaction('archivosContacto', 'readwrite');
                let storeArchivos = transaction.objectStore('archivosContacto');
                let request = storeArchivos.delete(fileKey);
                request.onsuccess = () => {
                    resolve();
                }
                request.onerror = () => {
                    console.error('Error al borrar archivo', request.error);
                    reject(request.error);
                }
            });
        });
    }

    borrarTodosArchivosContacto(): Promise<void> {
        return this.dbReady.then(() => {
            return new Promise<void>((resolve, reject) => {
                let transaction = this.db.transaction('archivosContacto', 'readwrite');
                let storeArchivos = transaction.objectStore('archivosContacto');
                let request = storeArchivos.clear();
                request.onsuccess = () => {
                    resolve();
                }
                request.onerror = () => {
                    console.error('Error al borrar todos los archivos', request.error);
                    reject(request.error);
                }
            });
        });
    }
}

