import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CanActivate, CanActivateChild, Router } from "@angular/router";
import { throwError } from 'rxjs';
import { catchError, timeout } from 'rxjs/operators';
import { SessionDataService } from "../services/SessionDataService";

@Injectable()
export class LoginGuard implements CanActivate, CanActivateChild {

    private readonly checkSessionIntervalMs: number = 15000;
    private readonly checkSessionRequestTimeoutMs: number = this.checkSessionIntervalMs - 300;
    private readonly defaultCheckSessionTimeoutMs: number = 1000 * 60 * 5; // 5 minutos, 60 segundos en cada minuto, 1000ms en cada segundo.
    private checkSessionRemainingTime: number = this.defaultCheckSessionTimeoutMs;

    constructor(private router: Router, private http: HttpClient, private sessionDataService: SessionDataService) { }
    checkActivated: boolean = false;

    canActivate() {
        this.checkSession();
        return this.checkIfLoggedIn();
    }

    canActivateChild() {
        setTimeout(() => {
            var url = this.router.url;
            if ((url.indexOf("/") != -1 || url.indexOf("/home") != -1 || url.indexOf("/home-ngs") != -1) && this.checkActivated == false) {
                this.checkActivated = true;
                this.checkSession();
            }
        }, 2000);

        return this.checkIfNeedLogIn();
    }

    public async getEstado(): Promise<any> {
        let headers = new HttpHeaders();
        headers.append('Content-Type', 'application/json');
        headers.append('Accept', 'q=0.8;application/json;q=0.9')
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');

        this.http.get<{ tieneSesion: boolean }>('/api/Home/VerificarEstadoSesion', { headers: headers })
            .pipe(
                timeout(this.checkSessionRequestTimeoutMs),
                catchError((error) => {
                    console.error('Error en la solicitud:', error);
                    this.checkSessionRemainingTime -= this.checkSessionIntervalMs;
                    if (this.checkSessionRemainingTime <= 0) {
                        this.sessionDataService.logout();
                        window.location.href = window.location.origin + '/SignOut';
                    }
                    return throwError(error);
                })
            )
            .subscribe((result: any) => {
                this.checkSessionRemainingTime = this.defaultCheckSessionTimeoutMs;
                if (!result.tieneSesion) {
                    this.sessionDataService.logout();
                    window.location.href = window.location.origin + '/SignOut';
                    return false;
                }
            });
    }
    private checkIfNeedLogIn(): boolean {

        let loggedIn: boolean = (sessionStorage.getItem("proveedor") != undefined && sessionStorage.getItem("proveedor") != "");

        this.getEstado();

        if (!loggedIn) {
            return false;
        }

        return true;
    }

    private checkIfLoggedIn(): boolean {

        let loggedIn: boolean = (sessionStorage.getItem("proveedor") != undefined && sessionStorage.getItem("proveedor") != "");
        this.getEstado();

        if (loggedIn) {
            if (sessionStorage.getItem("granosFlag") == "N") {
                this.router.navigate(['/home-ngs']);
            } else {
                this.router.navigate(['/home']);
            }
            return false;
        }

        return true;
    }

    private checkSession() {
        setTimeout(() => {
            this.checkIfNeedLogIn();
            this.checkSession();
        }, this.checkSessionIntervalMs);
    }


}