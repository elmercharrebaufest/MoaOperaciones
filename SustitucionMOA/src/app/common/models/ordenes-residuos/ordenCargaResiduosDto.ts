import { Almacen } from "../almacen";
import { LocalidadDto } from "../common/localidadDto";
import { Material } from "../material";
import { Proveedor } from "../proveedor";
import { Domicilio } from "./domicilio";
import { EstadoOrdenResiduos } from "./estadoOrdenResiduos";
import { Planta } from "./planta";

export class OrdenCargaResiduosDto {

    public Id: number;
    public Estado: EstadoOrdenResiduos;
    public Cliente: Proveedor;

    public Producto: Material;

    public Localidad?: LocalidadDto;
    public Planta?: Planta;
    public Domicilio?: Domicilio;
    public Almacen: Almacen;

    public PatenteChasis: string;
    public PatenteAcoplado: string;
    public NombreChofer: string;
    public ApellidoChofer: string;
    public CUILChofer: string;
    public RazonSocialTransporte: string;
    public CUITTransporte: string;
    public Observaciones: string;
    public FechaCreacion: string;
    public FechaIngreso: string;
    public FechaEgreso: string;

    public CantidadDeViajes?: number;

    constructor() {
        this.Cliente = new Proveedor();
        this.Producto = new Material();
        this.Localidad = new LocalidadDto();
        this.Estado = new EstadoOrdenResiduos();
        this.Almacen = new Almacen();
    }
}