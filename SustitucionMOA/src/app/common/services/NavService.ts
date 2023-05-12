import { Injectable } from '@angular/core';
import { Router } from "@angular/router";
import { Subject } from 'rxjs';
import { Seccion } from './../models/seccion';

@Injectable()
export class NavService {

    constructor(private router: Router) { }

    public seccionList = new Subject<Seccion[]>();
    public seccionActive = new Subject<string>();
    public menuActive = new Subject<string>();

    seccionList$ = this.seccionList.asObservable();
    seccionActive$ = this.seccionActive.asObservable();
    menuActive$ = this.menuActive.asObservable();

    public menuActiveValue = "";
    public seccionActiveValue = "";

    navegarSeccion(path: string) {
        this.router.navigate([path]);
    }

    navegarSeccionParam(path: string, param: string) {
        this.router.navigate([path, param]);
    }

    navegarSeccionParamDos(path: string, param1: string, param2:string) {
        this.router.navigate([path, param1, param2 ]);
    }

    navegarSeccionParamTres(path: string, param1: string, param2: string, param3: string) {
        this.router.navigate([path, param1, param2, param3]);
    }
    navegarBasic(path: string, rawParams?: Array<string> | string, queryParams?: {[key: string]: string}){
        let route = [path]
        if(rawParams){
            if(typeof rawParams == 'string')
                route.push(rawParams)
            else 
                route = route.concat(rawParams)
        }
        this.router.navigate(route,{queryParams});
    }

    setMenuSeccionTab(menu: string, seccion: string) {
        this.setMenuActive(menu);
        this.setSeccionActive(seccion);
    }

    setSeccionList(seccionList: Seccion[]) {
        this.seccionList.next(seccionList);
    }

    setSeccionActive(value: string) {
        this.seccionActive.next(value);
        this.seccionActiveValue = value;
    }

    setMenuActive(value: string) {
        this.menuActive.next(value);
        this.menuActiveValue = value;
    }
}