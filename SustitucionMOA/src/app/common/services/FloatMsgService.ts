import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class FloatMsgService {

    constructor() { }

    public errorMsj = new Subject<any>();
    public infoMsj = new Subject<any>();
    public successMsj = new Subject<any>();


    errorMsj$ = this.errorMsj.asObservable();
    infoMsj$ = this.infoMsj.asObservable();
    successMsj$ = this.successMsj.asObservable();

    setMsgsEmpty() {
        this.errorMsj.next("");
    }

    setErrorMsg(value: any) {
        this.errorMsj.next(value);
    }

    setInfoMsg(value: any) {
        this.infoMsj.next(value);
    }

    setSuccessMsg(value: any) {
        this.successMsj.next(value);
    }

}