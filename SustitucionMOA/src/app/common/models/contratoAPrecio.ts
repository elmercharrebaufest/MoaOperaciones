export class ContratoAPrecio {
    public TipoNegocioId: number;
    public Id: number;
    public MaterialId: number;
    public Cantidad: number;
    public Precio: number;
    public PrecioNeto: number;
    public CampanaId: number;
    public MonedaId: string;
    public ComercialId: number;
    public ContratoSAP: string;
    public PorcentajeDePago: number;
    public ComercialCreadorId: number;
    public PagoDiferidoTercero: boolean;
    public DolarizadoTercero: boolean;
    public EstadoId: number;
    public ClasificacionId: number;
    public Observacion: string;
    public CondicionFijacionId: number;
    public DestinoId: number;
    public BoletoId: number;
    public BolsaId: number;
    public ProveedorId: number;
    public CorredorId: number;
    public LocalidadId: number;
    public ProvinciaId: number;
    public StandardDeCalidadId: number;
    public FechaOperacion: Date;
    //public DesdeFijacion: Date;
    //public HastaFijacion: Date;
    public FechaEntrega: Date;
    public FechaDesde: Date;
    public Fecha: Date;
    public FechaHasta: Date;
    public EstablecimientoPropio: boolean;
    public ZonaId: boolean;
    public Consignatario: boolean;
    public PlanCanje: boolean;
    public ObservacionTercero: string;
    public CalidadTercero: boolean;
    public Pizarra: boolean;
    public SustentableTercero: boolean;
    public ContratoCorredor: string;
    public ContratoVendedor: string;
    public CantidadCamiones: number;
    public DiasPesificado: number;

    constructor() {
        this.TipoNegocioId = 2;
        this.Id = 0;
    }
}
