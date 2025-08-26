import { UnidadTransporteCarga } from "../ordenes-de-carga-common/unidadTransporteCarga";
import { OrdenDeCargaFasonDto } from "./ordenDeCargaFasonDto";

export class CrearOrdenDeCargaFasonRequest extends OrdenDeCargaFasonDto {
    
    public UnidadesTransporte: UnidadTransporteCarga[] = [];
}