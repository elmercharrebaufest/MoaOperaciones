import { HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, throwError } from "rxjs";
import { map, timeoutWith } from "rxjs/operators";
import { CartaPresentacion } from "../../common/models/cartaPresentacion";
import { InformeComercial } from "../../common/models/informeComercial";
import { BaseService } from "./../../common/services/BaseService";
import { ApiResponse } from "../../common/models/response";

@Injectable()
export class EmpresaGranosService extends BaseService {
    postFile(
        files: FileList,
        fileKey: string,
        proveedorId: number
    ): Observable<any> {
        let formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            let fileToUpload = files.item(i);
            formData.append("file", fileToUpload, fileToUpload.name);
        }

        formData.append("fileKey", fileKey);
        formData.append("proveedorId", proveedorId.toString());

        return this.http
            .post("/api/AltaEmpresaGranos/GuardarArchivo", formData);
    }

    searchLocalidad(term): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");

        let params: HttpParams = new HttpParams();
        params = params.set("localidad", term);

        return this.http
            .get("/api/AltaEmpresaGranos/GetLocalidadCombo", {
                params: params,
                headers: headers,
            });
    }

    generarInformeComercial(
        informeComercial: InformeComercial,
        mailUsuario: string,
        proveedorId?: number
    ): Observable<any> {
        let payload = new FormData();
        payload.append(
            "informeComercialJson",
            JSON.stringify(informeComercial)
        );

        payload.append("proveedorId", proveedorId.toString());
        payload.append("mailUsuario", mailUsuario);

        return this.http
            .post("/api/AltaEmpresaGranos/GenerarInformeComercial", payload)
            .pipe(
                timeoutWith(
                    30000,
                    throwError(
                        new Error(
                            "Se excedió el tiempo de espera, por favor inténtelo más tarde "
                        )
                    )
                )
            );
    }

    generarCartaPresentacion(
        cartaPresentacion: CartaPresentacion,
        proveedorId?: number,
    ): Observable<any> {
        let payload = new FormData();
        payload.append(
            "cartaPresentacionJson",
            JSON.stringify(cartaPresentacion)
        );
        payload.append("proveedorId", proveedorId.toString());

        return this.http
            .post("/api/AltaEmpresaGranos/GenerarCartaPresentacion", payload)
            .pipe(
                timeoutWith(
                    30000,
                    throwError(
                        new Error(
                            "Se excedió el tiempo de espera, por favor inténtelo más tarde "
                        )
                    )
                )
            );
    }

    obtenerMateriales(): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");

        return this.http
            .get("/api/AltaEmpresaGranos/GetMateriales", {
                headers: headers,
            });
    }

    obtenerCampanias(): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");

        return this.http
            .get("/api/AltaEmpresaGranos/GetCampanias", {
                headers: headers,
            });
    }

    obtenerArchivosSubidos(
        mail?: string,
        proveedorId?: number
    ): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");
        let params: HttpParams = new HttpParams();
        params = params.append("mail", mail);
        params = params.append("proveedorId", proveedorId.toString());
        return this.http
            .get("/api/AltaEmpresaGranos/ObtenerArchivosSubidos", {
                params: params,
                headers: headers,
            });
    }

    descargarArchivosSubidos(
        mail: string,
        proveedorId: number
    ): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");

        let params: HttpParams = new HttpParams();
        params = params.append("mail", mail);
        params = params.append("proveedorId", proveedorId.toString());
        return this.http
            .get("/api/AltaEmpresaGranos/DescargarArchivos", {
                params: params,
                headers: headers,
            });
    }

    obtenerInfoProveedor(proveedorId: number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append("mail", "");
        params = params.append("proveedorId", proveedorId.toString());

        return this.http
            .get("/api/AltaEmpresaGranos/ObtenerInfoProveedor", {
                params: params,
                headers: this.headers,
            });
    }

    descargarArchivoSubido(
        fileKey: string,
        archivoID?: number,
        proveedorId?: number
    ): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append("mail", "");
        params = params.append("archivoID", archivoID.toString());
        params = params.append("proveedorId", proveedorId.toString());

        return this.http
            .get("/api/AltaEmpresaGranos/DescargarArchivo", {
                params: params,
                headers: this.headers,
            });
    }

    enviarSolicitud(datos: any, proveedorId: number): Observable<any> {
        let payload = new FormData();
        payload.append("datosJson", JSON.stringify(datos));
        payload.append("proveedorId", proveedorId.toString());

        return this.http
            .post("/api/AltaEmpresaGranos/EnviarSolicitudUsuario", payload)
            .pipe(
                timeoutWith(
                    30000,
                    throwError(
                        new Error(
                            "Se excedió el tiempo de espera, por favor inténtelo más tarde "
                        )
                    )
                )
            );
    }

    eliminarArchivoSubido(
        archivoID?: number,
        proveedorId?: number
    ): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append("archivoID", archivoID.toString());
        params = params.append("proveedorId", proveedorId.toString());

        return this.http
            .get("/api/AltaEmpresaGranos/EliminarArchivo", {
                params: params,
                headers: this.headers,
            });
    }

    cargarSolicitudUsuario(
        mail?: string,
        proveedorId?: number
    ): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");
        let params: HttpParams = new HttpParams();
        params = params.append("mail", mail);

        if (proveedorId)
            params = params.append("proveedorId", proveedorId.toString());
        else
            params = params.append("proveedorId", "0");

        return this.http
            .get("/api/AltaEmpresaGranos/CargarSolicitudUsuario", {
                params: params,
                headers: this.headers,
            });
    }

    descargarFormularioNG(empresaId: number): Observable<any> {
        let payload = new FormData();

        payload.append("empresaId", empresaId.toString());

        return this.http
            .post("/api/AltaEmpresaNoGranos/DescargarFormularioNG", payload)
            .pipe(
                timeoutWith(
                    30000,
                    throwError(
                        new Error(
                            "Se excedió el tiempo de espera, por favor inténtelo más tarde "
                        )
                    )
                )
            );
    }

    public getRubros(): Observable<any> {
        return this.http
            .get('/api/usuario/getRubros', { headers: this.headers });
    }

    public getTipoCambiario(): Observable<any> {
        return this.http
            .get('/api/dataagro/GetTipoCambiario', { headers: this.headers });
    }

    public editarProveedorNoGranos(razonSocial: any, cuit: any, email: any, telefono: any,
        realizarAnalisisNOSIS: any, IdRubro: any, condicionDePago: any, servicioPrestado: any,
        organizacionDeCompra: any, razonDeEleccion: any, facturacionAnual: any, idProveedor: any,
        requiereVerificacionCompras: any, ingresoAPlanta: any, altaInterna: any, siperObligatorio: any) {

        let params: HttpParams = new HttpParams();
        params = params.append('razonSocial', razonSocial);
        params = params.append('cuit', cuit);
        params = params.append('email', email);
        params = params.append('telefono', telefono);
        params = params.append('realizarAnalisisNOSIS', realizarAnalisisNOSIS);
        params = params.append('IdRubro', IdRubro);
        params = params.append('condicionDePago', condicionDePago);
        params = params.append('servicioPrestado', servicioPrestado);
        params = params.append('organizacionDeCompra', organizacionDeCompra);
        params = params.append('razonDeEleccion', razonDeEleccion);
        params = params.append('facturacionAnual', facturacionAnual);
        params = params.append('proveedorId', idProveedor);
        params = params.append('requiereVerificacionCompras', requiereVerificacionCompras);
        params = params.append('ingresoAPlanta', ingresoAPlanta);
        params = params.append('altaInterna', altaInterna);
        params = params.append('siperObligatorio', siperObligatorio);

        return this.http.get('/api/AltaEmpresaNoGranos/EditarProveedorNoGranos', { params: params, headers: this.headers });
    }

    public registrarDocumentacionFisica(proveedorId: number, contieneDocumentacionFisica: boolean) {
        let params = {
            proveedorId: proveedorId.toString(),
            contieneDocumentacionFisica: contieneDocumentacionFisica ? 'true' : 'false'
        }

        return this.http.get('/api/AltaEmpresaNoGranos/RegistrarDocumentacionFisica', { params: params, headers: this.headers });
    }

    SolicitudAltaInterna(proveedorId: number, datos: any): Observable<any> {
        let datosJson = JSON.stringify(datos);
        var payload = new FormData();

        payload.append('datosJson', datosJson);
        payload.append('proveedorId', proveedorId.toString());

        return this.http
            .post("/api/AltaEmpresaGranos/SolicitudAltaInterna", payload, { headers: this.headers })
            .pipe(
                timeoutWith(
                    30000,
                    throwError(
                        new Error(
                            "Se excedió el tiempo de espera, por favor inténtelo más tarde "
                        )
                    )
                )
            );
    }

    modificarEstadoProveedor(proveedorId: number, nuevoEstado: string): Observable<ApiResponse<{ nuevoEstado: number }>> {
        let params = {
            proveedorId: proveedorId.toString(),
            nuevoEstado: nuevoEstado
        }

        return this.http.get<ApiResponse<{ nuevoEstado: number }>>('/api/AltaEmpresa/ModificarEstadoProveedor', { params: params, headers: this.headers });

    }
}
