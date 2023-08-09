export class CampoObligatorioViewModel
{
    public nombreCampo : string;
    public isValid : boolean;
    public tieneValorPorDefecto : boolean;
    public valorPorDefecto : any;

    constructor(nombreCampo:string, tieneValorPorDefecto:boolean = false, valorPorDefecto:any = null)
    {
        this.nombreCampo = nombreCampo;
        this.tieneValorPorDefecto = tieneValorPorDefecto;
        this.valorPorDefecto = valorPorDefecto;
        this.isValid = true;
    }

}