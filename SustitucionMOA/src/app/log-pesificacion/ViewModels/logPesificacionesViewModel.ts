export class LogPesificacionViewModel
{
    public id : number;
    public fecha : string;
    public contrato :number;
    public fijacion :number;
    public cantidadKilos : number;
    public usuario : string;
    public proveedor : string;
    public rutaFisicaArchivo : string;
    public idArchivo :number;
    public estado : string;

    constructor(responseDto : any = undefined)
    {
        if(responseDto == undefined)
        {
            this.id = 0;
            this.fecha = "";
            this.contrato = 0;
            this.fijacion = 0;
            this.cantidadKilos = 0;
            this.usuario="";
            this.rutaFisicaArchivo="";
            this.idArchivo = 0;
            this.estado="";
        }
        else
        {
            this.id = responseDto.Id;
            this.fecha = responseDto.Fecha;
            this.contrato = responseDto.Contrato;
            this.cantidadKilos = responseDto.CantidadKilos;
            this.fijacion =responseDto.Fijacion;
            this.usuario = responseDto.Usuario;
            this.proveedor = responseDto.Proveedor;
            this.rutaFisicaArchivo = responseDto.RutaFisicaArchivo
            this.idArchivo = responseDto.IdArchivo;
            this.estado = responseDto.Estado;
        }
    }

}