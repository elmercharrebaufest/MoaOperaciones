import { UnidadTransporteCarga } from "../ordenes-de-carga-common/unidadTransporteCarga";
import { OrdenDeCargaFasonDto } from "./ordenDeCargaFasonDto";

export class EditarOrdenDeCargaFasonRequest extends OrdenDeCargaFasonDto {

    public UnidadTransporte: UnidadTransporteCarga;
}