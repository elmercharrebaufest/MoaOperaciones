import { CanActivate, CanActivateChild } from "@angular/router";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { SessionDataService } from "../services/SessionDataService";
import { HttpClient, HttpHeaders } from "@angular/common/http";

@Injectable()
export class LoginGuard implements CanActivate, CanActivateChild {


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

        await this.http.get<{ tieneSesion: boolean }>('/api/Home/VerificarEstadoSesion', { headers: headers }).subscribe(
            (result: any) => {
                if (!result.tieneSesion) {
                    this.sessionDataService.logout();
                    window.location.href = window.location.origin + '/SignOut';
                    return false;
                }
            }
        )
    }
    private checkIfNeedLogIn(): boolean {

        let loggedIn: boolean = (sessionStorage.getItem("proveedor") != undefined && sessionStorage.getItem("proveedor") != "");

        this.getEstado();

        if (!loggedIn) {
            this.router.navigate(['login']);
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
        }, 15000);
    }


}