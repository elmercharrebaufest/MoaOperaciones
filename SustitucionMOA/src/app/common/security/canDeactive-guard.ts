import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanDeactivate } from "@angular/router";

@Injectable({
    providedIn: 'root'
})
export class ConfirmDeactivateService {
    private canDeactivate = false;

    getState() {
        return this.canDeactivate;
    }
    setState(state = false) {
        this.canDeactivate = state;
    }
}

@Injectable({
    providedIn: 'root'
})
export class ConfirmDeactivated implements CanDeactivate<any> {

    component: any;
    route: ActivatedRouteSnapshot;

    constructor(protected confirmDeactivateService: ConfirmDeactivateService) {
    }

    canDeactivate() {
        if (!this.confirmDeactivateService.getState())
            return window.confirm('Está por dejar la pantalla sin terminar, de hacerlo pueden haber errores. ¿Seguro que desea dejar la pantalla?');

        return true;
    }

}