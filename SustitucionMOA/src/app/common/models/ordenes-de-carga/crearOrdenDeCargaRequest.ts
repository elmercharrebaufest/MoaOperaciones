import { UnidadTransporteCarga } from "../ordenes-de-carga-common/unidadTransporteCarga";
import { GestionAltasFAS } from "./gestionAltasFAS";
import { OrdenDeCarga } from "./ordenDeCarga";

export class CrearOrdenDeCargaRequest {

    public OrdenDeCarga: OrdenDeCarga;

    public GestionAltasFAS: GestionAltasFAS;

    public UnidadesTransporte: UnidadTransporteCarga[] = [];
}