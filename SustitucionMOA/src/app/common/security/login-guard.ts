import { CanActivate, CanActivateChild } from "@angular/router";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";

@Injectable()
export class LoginGuard implements CanActivate, CanActivateChild {

    constructor(private router: Router) { }

    canActivate() {
        return this.checkIfLoggedIn();
    }

    canActivateChild() {
        return this.checkIfNeedLogIn();
    }

    private checkIfNeedLogIn(): boolean {

        let loggedIn: boolean = (sessionStorage.getItem("proveedor") != undefined && sessionStorage.getItem("proveedor") != "");

        if (!loggedIn) {
            this.router.navigate(['login']);
            return false;
        }

        return true;
    }

    private checkIfLoggedIn(): boolean {

        let loggedIn: boolean = (sessionStorage.getItem("proveedor") != undefined && sessionStorage.getItem("proveedor") != "");

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