import { Injectable } from '@angular/core';
import { Router } from "@angular/router";

@Injectable()
export class SecurityService {

    constructor(private router: Router) { }

    tienePermiso(permiso: string) {
        return (JSON.parse(sessionStorage.getItem("permisos")).indexOf(permiso) != -1)
    }

    tienePermisoRedirect(permiso: string) {
        if (sessionStorage.getItem("permisos")) {
            if (JSON.parse(sessionStorage.getItem("permisos")).indexOf(permiso) == -1) {
                this.router.navigate(['no-autorizado']);
            }
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