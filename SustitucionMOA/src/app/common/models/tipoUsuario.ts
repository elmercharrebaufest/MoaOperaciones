export class TipoUsuario {
    Id: string;
    Nombre: string;
    checked: boolean;

     public constructor(init?:Partial<TipoUsuario>) {
        Object.assign(this, init);
    }
}