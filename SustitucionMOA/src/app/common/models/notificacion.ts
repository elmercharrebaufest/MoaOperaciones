import { Rol } from "./rol";
import { TipoUsuario } from "./tipoUsuario";

export class Notificacion {
    public Id: number;
    public Nombre: string;
    public FechaInicio: Date;
    public FechaFin: Date;
    public HoraInicio: number;
    public Habilitada: boolean;
    public Borrada: boolean;
    public LinkAdjunto: string;
    public Mensaje: string;
    public FiltroRoles: Array<Rol>
    public FiltroTipoUsuario: Array<TipoUsuario>

    constructor() { 
        this.FiltroRoles = new Array<Rol>();
        this.FiltroTipoUsuario = new  Array<TipoUsuario>();
    }
}
