
export class Notificacion {
    public Id: number;
    public Nombre: string;
    public FechaInicio: Date;
    public FechaFin: Date;
    public Habilitada: boolean;
    public Borrada: boolean;
    public LinkAdjunto: string;
    public Mensaje: string;
    public FiltroRoles: Array<Number>
    public FiltroTipoUsuario: Array<Number>

    constructor() { 
        this.FiltroRoles = new Array<Number>();
        this.FiltroTipoUsuario = new  Array<Number>();
    }
}
