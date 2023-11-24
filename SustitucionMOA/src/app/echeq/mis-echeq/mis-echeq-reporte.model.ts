export class EcheqReporte {
    public RazonSocial: string; 
    public Mail: string; 
    public CodigoProveedor: string; 
    public Contrato: string;  
    public Liquidacion: string;  
    public LiquidacionMarcada: boolean; 
    public EcheqGenerados: boolean;  
    public FechaCreacion: string;  
    public CantidadDeEcheqs: number;  
    public MontosEcheqs: any[];  
    public ImporteTotal: number;  

    constructor() {}
}