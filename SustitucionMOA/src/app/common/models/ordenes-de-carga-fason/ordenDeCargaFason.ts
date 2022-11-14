
import { EstadoOrdenDeCarga } from "../ordenes-de-carga/estadoordendecarga";



export class OrdenDeCargaFason {
    Id: number;
    Estado: EstadoOrdenDeCarga;
    FechaCreacion: string;
    FechaRetiro: string;
    Cantidad: number;
    PatenteChasis: string;
    PatenteAcoplado: string;
    
    NombreChofer: string;
    CUILChofer: number;
    RazonSocialTransporte: string;
    CUITTransporte: number;
    Destino: string;
    CantidadDeViajesRealizados: number;
    CantidaDeViajesEsperados: number;
    Observacion: string;
    Cliente: number;
    Producto_Id: number;
    TransporteExiste: boolean;
    constructor() {
    }


}

