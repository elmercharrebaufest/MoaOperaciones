import { CanActivate, CanActivateChild } from "@angular/router";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { SessionDataService } from "../services/SessionDataService";

@Injectable()
export class LoginGuard implements CanActivate, CanActivateChild {


    constructor(private router: Router, private http: Http, private sessionDataService: SessionDataService) {

    }

    canActivate() {
        return this.checkIfLoggedIn();
    }

    canActivateChild() {
        return this.checkIfNeedLogIn();
    }

    public async getEstado(): Promise<any> {
        let params: URLSearchParams = new URLSearchParams();

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('Accept', 'q=0.8;application/json;q=0.9')
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');

        await this.http.get('/api/Home/VerificarEstadoSesion', { headers: headers }).subscribe(
            res => {
                let result = res.json()
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
}