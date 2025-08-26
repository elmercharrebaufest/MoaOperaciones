import { DestinoFason } from "../../../ordenes-de-carga-fason/orden-carga-fason-utils";
import { OrdenesBase } from "../../base-components/ordenes-base-component";
import { Material } from "../material";
import { EstadoOrdenDeCargaFason } from "./estadoOrdenDeCargaFason";

export class OrdenDeCargaFasonDto extends OrdenesBase {

    public Id: number;
    public Estado: EstadoOrdenDeCargaFason;

    public Corredor: string;
    public CUITCorredor?: string;
    public RazonSocialCorredor: string;
    public CorredorId?: number;
    public CodigoCorredor?: string;
    
    public CUITCliente: number;
    public Cliente: string;
    public RazonSocialCliente: string;

    public Producto_Id: number;
    public ProductoSeleccionado: Material;
    public Material: string;
    public ValidaSisaRuca: boolean;
    
    public CUITTercero: number;
    public Cantidad: number;
    public Destino: DestinoFason;
    public LocalidadDescripcion: string;
    
    public PatenteChasis: string;
    public PatenteAcoplado: string;
    public NombreChofer: string;
    public ApellidoChofer: string;
    public RazonSocialTransporte: string;
    public CantidadDeViajes: number;

    public Observacion: string;

    public FechaCreacion: string;
    public FechaRetiro: Date;
    public FechaRetiroReal?: Date;
    public FechaIngresoPlanta?: Date;
    public CantidadDescargada: number;
    public TransporteExiste: boolean;
    public TienePatentesRepetidas?: boolean;
    public TienePatenteMultiplesAutorizaciones?: boolean;
    public RemitenteComercial?: boolean;

    public OrdenesConPatentesRepetidas?: number[];
    
    constructor() {
        super();
        this.FechaRetiro = new Date();
        this.Cantidad = 30000;
    }

}


