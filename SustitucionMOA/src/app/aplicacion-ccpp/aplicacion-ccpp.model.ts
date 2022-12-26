import { DropdownOption } from '../common/view-child/dropdown/dropdown.component';
export enum EstadoAplicacionCCPP{
    Pendiente,
    Aplicado,
    Error
}
export interface AplicacionCCPP{}
export const SeccionAplicacionCCPP = "aplicaciones-ccpp";
export const estadosAplicacionCCPP: DropdownOption[] = [
    {value:"", label: "Todos"},
    {value:EstadoAplicacionCCPP.Aplicado.toString(), label: "Aplicado"},
    {value:EstadoAplicacionCCPP.Error.toString(), label: "Error"},
    {value:EstadoAplicacionCCPP.Pendiente.toString(), label: "Pendiente"},
] 