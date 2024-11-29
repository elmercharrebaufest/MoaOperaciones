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

                this.borrarArchivosDeOtrasConsultas(consultaId, storeArchivos)
                    .then(() => {
                        let archivoBD: ArchivoBD = {key: fileKey, archivo: archivo};

                        let requestAgregarArchivo = storeArchivos.add(archivoBD);

                        requestAgregarArchivo.onsuccess = () => {
                            resolve();
                        };
                        requestAgregarArchivo.onerror = () => {
                            console.error('Error al agregar al store el archivo', requestAgregarArchivo.error);
                            reject(requestAgregarArchivo.error);
                        };
                    });
            });
        });
    }

    obtenerArchivosContacto(consultaId: number): Promise<ArchivoBD[]> {
        return this.dbReady.then(() => {
            return new Promise<ArchivoBD[]>((resolve, reject) => {
                let transaction = this.db.transaction('archivosContacto', 'readonly');
                let storeArchivos = transaction.objectStore('archivosContacto');
                let requestArchivos = storeArchivos.getAll(IDBKeyRange.bound(consultaId.toString(), (consultaId + 1).toString()));
                
                requestArchivos.onsuccess = () => {
                    let archivosBD = requestArchivos.result as ArchivoBD[];
                    resolve(archivosBD);
                }
                requestArchivos.onerror = () => {
                    console.error('Error al intentar obtener los archivos de consulta ' + consultaId, requestArchivos.error);
                    reject(requestArchivos.error);
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
                let requestBorrarArch = storeArchivos.delete(fileKey);
                requestBorrarArch.onsuccess = () => {
                    resolve();
                };
                requestBorrarArch.onerror = () => {
                    console.error('Error al borrar archivo', requestBorrarArch.error);
                    reject(requestBorrarArch.error);
                };
            });
        });
    }

    borrarTodosArchivosContacto(): Promise<void> {
        return this.dbReady.then(() => {
            return new Promise<void>((resolve, reject) => {
                let transaction = this.db.transaction('archivosContacto', 'readwrite');
                let storeArchivos = transaction.objectStore('archivosContacto');
                let requestBorrarArchivos = storeArchivos.clear();
                requestBorrarArchivos.onsuccess = () => {
                    resolve();
                }
                requestBorrarArchivos.onerror = () => {
                    console.error('Error al borrar todos los archivos', requestBorrarArchivos.error);
                    reject(requestBorrarArchivos.error);
                }
            });
        });
    }

    private borrarArchivosDeOtrasConsultas(consultaId: number, storeArchivos: IDBObjectStore): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            let requestKeys = storeArchivos.getAllKeys(IDBKeyRange.bound(consultaId.toString(), (consultaId + 1).toString()));
            requestKeys.onerror = () => {
                console.error('Error al obtener las keys para consulta ' + consultaId, requestKeys.error);
                reject(requestKeys.error);
            };
            requestKeys.onsuccess = () => {
                let keysContacto = requestKeys.result;
                if (keysContacto && keysContacto.length > 0) {
                    // Si ya hay algún archivo para esta Consulta, no hacemos nada más (los de Consultas previas ya habían sido borrados)
                    resolve();
                }
                else {
                    // Si no se encuentra ninguna key para este número de Consulta, se trata del primer archivo que se le adjunta.
                    // Antes de grabarlo, borramos los registros que puedan existir de Consultas previas
                    let requestBorrarPrevios = storeArchivos.clear();
                    requestBorrarPrevios.onerror = () => {
                        console.error('Error al borrar los archivos de Consultas distintas a ' + consultaId, requestBorrarPrevios.error);
                        reject(requestBorrarPrevios.error);
                    }
                    requestBorrarPrevios.onsuccess = () => {
                        resolve();
                    }
                }
            }
        });
    }
}

