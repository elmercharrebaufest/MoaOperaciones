import { CommonResponse } from "./common-response";

export class UsuarioLogueado extends CommonResponse {
    public success;
    public username;
    public nombre;
    public proveedor;
    public granosFlag;
    public permisos;
    public tipoUsuario;
    public noticias;
    public esNuevoUsuario;
    public redirectURL;
    public seccionesVisitadas;
    public aceptoTyC;
    public apikey;
    public url;
}