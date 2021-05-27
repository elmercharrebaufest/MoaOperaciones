export interface IValidadorPasoSolp
{
    esPasoInvalido() : boolean

    aplicarValidaciones() : void

    mostrarError(nombreCampo : string): boolean
}