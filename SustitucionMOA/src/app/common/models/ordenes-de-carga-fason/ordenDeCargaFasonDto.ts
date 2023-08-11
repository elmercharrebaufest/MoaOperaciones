import { DestinoFason } from "../../../ordenes-de-carga-fason/orden-carga-fason-utils";
import { OrdenesBase } from "../../base-components/ordenes-base-component";
import { Material } from "../material";
import { EstadoOrdenDeCargaFason } from "./estadoOrdenDeCargaFason";

export class OrdenDeCargaFasonDto extends OrdenesBase {

    public Id: number;
    public CUITTercero: number;
    public Estado: EstadoOrdenDeCargaFason;
    public Producto_Id: number;
    public ProductoSeleccionado: Material;
    public Material: string;
    public ValidaSisaRuca: boolean;

    public CUITCliente: number;
    public Cliente: string;
    public RazonSocialCliente: string;

    public Corredor: string;
    public CUITCorredor?: string;
    public RazonSocialCorredor: string;
    public CorredorId?: number;

    public FechaCreacion: string;
    public FechaRetiro: Date;
    public Cantidad: number;
    public PatenteAcoplado: string;
    public PatenteChasis: string;
    public NombreChofer: string;
    public CUILChofer: string;
    public RazonSocialTransporte: string;
    public CUITTransporte: string;
    public CantidadDeViajes: number;
    public Destino: DestinoFason;
    public TransporteExiste: boolean;
    public Observacion: string;
    public LocalidadDescripcion: string;

    constructor() {
        super();
        this.FechaRetiro = new Date();
        this.Cantidad = 30000;
    }

}


