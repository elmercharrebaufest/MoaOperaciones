import { Injectable } from "@angular/core";
import {
    HttpEvent,
    HttpInterceptor,
    HttpHandler,
    HttpRequest,
    HttpErrorResponse,
} from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import { Router } from "@angular/router";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private router: Router) {}

    intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError((error: HttpErrorResponse) => {
                let errorMsg =
                    "Ocurrió un error inesperado, por favor inténtelo más tarde";
                if (error.status === 500) {
                    errorMsg =
                        "Error del servidor, por favor inténtelo más tarde.";
                } else if (error.status === 404) {
                    errorMsg = "El recurso solicitado no fue encontrado.";
                }

                const customError = new Error(errorMsg);
                return throwError(customError);
            })
        );
    }
}
