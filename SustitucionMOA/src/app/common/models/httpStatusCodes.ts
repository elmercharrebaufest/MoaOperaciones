export class HttpStatusCodes {
    static readonly OK = 200;
    static readonly Created = 201;
    static readonly NoContent = 204;
    static readonly BadRequest = 400;
    static readonly Unauthorized = 401;
    static readonly Forbidden = 403;
    static readonly NotFound = 404;
    static readonly ServerError = 500;
    static msjError: string;

    static friendlyStatusCode(statusCode: number): string {
        switch (statusCode) {
            case this.BadRequest:
                this.msjError = 'La solicitud no pudo ser procesada.';
                break;
            case this.Unauthorized:
                this.msjError = 'Acceso no autorizado. Por favor, inicia sesión para acceder a esta página o recurso.';
                break;
                case this.Forbidden:
                    this.msjError = 'Usted no tiene permisos para acceder a la página que está intentando ver.';
                    break;
            case this.NotFound:
                this.msjError = 'La página que está buscando no se encuentra en nuestro sitio web.';
                break;
            case this.ServerError:
                this.msjError = 'Lo sentimos, ha ocurrido un problema en nuestro servidor.';
                break;
            default: this.msjError = 'Error inesperado:' + statusCode;
        }
        return this.msjError;
    }
}
