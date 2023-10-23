import { Injectable } from "@angular/core";
import { BaseService } from "../common/services/BaseService";

@Injectable({
    providedIn: 'root'
})
export class SendDataService extends BaseService {
    private data: any;

    setData(data: any){
        this.data = data;
    }

    getData(){
        return this.data;
    }

    limpiarData(){
        this.data = null;
    }
}