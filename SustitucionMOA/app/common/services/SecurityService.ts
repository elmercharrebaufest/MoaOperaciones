import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Router } from "@angular/router";
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/timeoutWith';
import 'rxjs/add/observable/throw';
import "rxjs/add/observable/defer";
import { Formatter } from './../formatter/Formatter';

@Injectable()
export class SecurityService {

    constructor(private router: Router) { }

    tienePermiso(permiso: string) {
        return (JSON.parse(sessionStorage.getItem("permisos")).indexOf(permiso) != -1)
    }

    tienePermisoRedirect(permiso: string) {
        if (JSON.parse(sessionStorage.getItem("permisos")).indexOf(permiso) == -1) {
            this.router.navigate(['no-autorizado']);
        }
    }

    esGranosRedirect() {
        if (sessionStorage.getItem("granosFlag") == "N") {
            this.router.navigate(['/home-ngs']);
            return false;
        }
        return true;
    }

    esNoGranosRedirect() {
        if (sessionStorage.getItem("granosFlag") == "G") {
            this.router.navigate(['/home']);
            return false;
        }
        return true;
    }

}