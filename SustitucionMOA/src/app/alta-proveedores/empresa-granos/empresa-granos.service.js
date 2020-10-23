var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Injectable } from "@angular/core";
import { URLSearchParams } from "@angular/http";
import { throwError } from "rxjs";
import "rxjs/add/observable/throw";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/map";
import { map, timeoutWith } from "rxjs/operators";
import { BaseService } from "./../../common/services/BaseService";
var EmpresaGranosService = /** @class */ (function (_super) {
    __extends(EmpresaGranosService, _super);
    function EmpresaGranosService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    EmpresaGranosService.prototype.postFile = function (files, fileKey, proveedorId) {
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            var fileToUpload = files.item(i);
            formData.append("file", fileToUpload, fileToUpload.name);
        }
        formData.append("fileKey", fileKey);
        formData.append("proveedorId", proveedorId.toString());
        return this.http
            .post("/api/AltaEmpresaGranos/GuardarArchivo", formData)
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.searchLocalidad = function (term) {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");
        var params = new URLSearchParams();
        params.set("localidad", term);
        return this.http
            .get("/api/AltaEmpresaGranos/GetLocalidadCombo", {
            search: params,
            headers: this.headers,
        })
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.generarInformeComercial = function (informeComercial, proveedorId) {
        var payload = new FormData();
        payload.append("informeComercialJson", JSON.stringify(informeComercial));
        payload.append("proveedorId", proveedorId.toString());
        return this.http
            .post("/api/AltaEmpresaGranos/GenerarInformeComercial", payload)
            .pipe(timeoutWith(30000, throwError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.generarCartaPresentacion = function (cartaPresentacion, proveedorId) {
        var payload = new FormData();
        payload.append("cartaPresentacionJson", JSON.stringify(cartaPresentacion));
        payload.append("proveedorId", proveedorId.toString());
        return this.http
            .post("/api/AltaEmpresaGranos/GenerarCartaPresentacion", payload)
            .pipe(timeoutWith(30000, throwError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.obtenerMateriales = function () {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");
        return this.http
            .get("/api/AltaEmpresaGranos/GetMateriales", {
            headers: this.headers,
        })
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.obtenerArchivosSubidos = function (mail, proveedorId) {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");
        var params = new URLSearchParams();
        params.set("mail", mail);
        params.set("proveedorId", proveedorId.toString());
        return this.http
            .get("/api/AltaEmpresaGranos/ObtenerArchivosSubidos", {
            search: params,
            headers: this.headers,
        })
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.obtenerInfoProveedor = function (proveedorId) {
        var params = new URLSearchParams();
        params.set("mail", "");
        params.set("proveedorId", proveedorId.toString());
        return this.http
            .get("/api/AltaEmpresaGranos/ObtenerInfoProveedor", {
            search: params,
            headers: this.headers,
        })
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.descargarArchivoSubido = function (fileKey, archivoID, proveedorId) {
        var params = new URLSearchParams();
        params.set("mail", "");
        params.set("archivoID", archivoID.toString());
        params.set("proveedorId", proveedorId.toString());
        return this.http
            .get("/api/AltaEmpresaGranos/DescargarArchivo", {
            search: params,
            headers: this.headers,
        })
            .pipe(map(this.extractData));
    };
    //enviarSolicitud(): Observable<any> {
    //    let params: URLSearchParams = new URLSearchParams();
    //    return this.http
    //        .get('/api/AltaEmpresaGranos/EnviarSolicitudUsuario', { search: params, headers: this.headers })
    //        .pipe(map(this.extractData));
    //}
    EmpresaGranosService.prototype.enviarSolicitud = function (datos, proveedorId) {
        var payload = new FormData();
        payload.append("datosJson", JSON.stringify(datos));
        payload.append("proveedorId", proveedorId.toString());
        return this.http
            .post("/api/AltaEmpresaGranos/EnviarSolicitudUsuario", payload)
            .pipe(timeoutWith(30000, throwError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.eliminarArchivoSubido = function (archivoID, proveedorId) {
        var params = new URLSearchParams();
        params.set("archivoID", archivoID.toString());
        params.set("proveedorId", proveedorId.toString());
        return this.http
            .get("/api/AltaEmpresaGranos/EliminarArchivo", {
            search: params,
            headers: this.headers,
        })
            .pipe(map(this.extractData));
    };
    EmpresaGranosService.prototype.cargarSolicitudUsuario = function (mail, proveedorId) {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");
        var params = new URLSearchParams();
        params.set("mail", mail);
        if (proveedorId)
            params.set("proveedorId", proveedorId.toString());
        else
            params.set("proveedorId", "0");
        return this.http
            .get("/api/AltaEmpresaGranos/CargarSolicitudUsuario", {
            search: params,
            headers: this.headers,
        })
            .pipe(map(this.extractData));
    };
    EmpresaGranosService = __decorate([
        Injectable()
    ], EmpresaGranosService);
    return EmpresaGranosService;
}(BaseService));
export { EmpresaGranosService };
//# sourceMappingURL=empresa-granos.service.js.map