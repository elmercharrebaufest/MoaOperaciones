import { Component, OnDestroy } from '@angular/core';
import { NavService } from './../services/NavService';
import { SecurityService } from './../services/SecurityService';
import { FloatMsgService } from './../services/FloatMsgService';
import { ModalService } from './../services/ModalService';

@Component({
    selector: 'app-base',
    template: ``
})
export class BaseComponent implements OnDestroy {

    constructor(protected navService: NavService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
    }

    subscription: any;
    subscriptionDropDowns: any;
    tipoUsuario: string = sessionStorage.getItem("tipoUsuario");

    public ngOnDestroy() {
        this.modalService.close();
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
    }

    public goToSeccion(path: string) {
        this.navService.navegarSeccion(path);
        return false;
    }

    public goToSeccionParam(path: string, param: string) {
        this.navService.navegarSeccionParam(path, param);
        return false;
    }

    public goToSeccionParamDos(path: string, param: string, param2: string) {
        this.navService.navegarSeccionParamDos(path, param, param2);
        return false;
    }
    public goToSeccionParamTres(path: string, param: string, param2: string, param3: string) {
        this.navService.navegarSeccionParamTres(path, param, param2, param3);
        return false;
    }
    public goToSimpleNavigation(path: string, rawParams?: Array<string> | string, queryParams?: { [key: string]: string }) {
        this.navService.navegarBasic(path, rawParams, queryParams);
        return false;
    }

    public setMenuSeccionTab(menu: string, seccion: string) {
        this.navService.setMenuSeccionTab(menu, seccion);
        return false;
    }

    public unsubscribe() {
        if (this.subscription != undefined)
            this.subscription.unsubscribe();
        if (this.subscriptionDropDowns != undefined)
            this.subscriptionDropDowns.unsubscribe();
    }

    public getDateFromAspNetFormat(date: string): number {
        if (date) {
            const re = /-?\d+/;
            const m = re.exec(date);
            return parseInt(m[0], 10);
        }
        return null
    }

    public convertDate(date: any) {
        var newDate = new Date(date),
            mnth = ("0" + (date.getMonth() + 1)).slice(-2),
            day = ("0" + date.getDate()).slice(-2);
        var hours = ("0" + date.getHours()).slice(-2);
        var minutes = ("0" + date.getMinutes()).slice(-2);
        var seconds = ("0" + date.getSeconds()).slice(-2);

        var mySQLDate = [day, mnth, date.getFullYear()].join("/");
        var mySQLTime = [hours, minutes, seconds].join(":");
        return [mySQLDate, mySQLTime].join(" ");
    }

    setTabs() { }

    isAuthorized(permiso: string) {
        return this.securityService.tienePermiso(permiso);
    }

    isCorredor() {
        return this.tipoUsuario.toUpperCase() == "CORR" || this.tipoUsuario.toUpperCase() == "NUECORR";
    }

    esCliente() {
        return this.tipoUsuario.toUpperCase() === "CLI";
    }

}