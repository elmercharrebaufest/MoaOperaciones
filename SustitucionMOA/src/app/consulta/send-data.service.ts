import { Injectable } from "@angular/core";
import { BaseService } from "../common/services/BaseService";

export interface DatosLiquidacionObservada {
    Tipo: "Parcial" | "Final";
    NroComprobante: number | string;
}

@Injectable({
    providedIn: 'root'
})
export class SendDataService extends BaseService {
    private data: any;

    private datosLiquidacionObservada?: DatosLiquidacionObservada

    setData(data: any) {
        this.data = data;
    }

    getData() {
        return this.data;
    }

    limpiarData() {
        this.data = null;
    }

    setDatosLiquidacionObservada(datosLiquidacion: DatosLiquidacionObservada) {
        this.datosLiquidacionObservada = datosLiquidacion;
    }
    getDatosLiquidacionObservada(): DatosLiquidacionObservada | null {
        return this.datosLiquidacionObservada;
    }
    limpiarDatosLiquidacionObservados() {
        this.datosLiquidacionObservada = null;
    }
}