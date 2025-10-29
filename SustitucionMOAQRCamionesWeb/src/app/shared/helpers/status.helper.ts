import { Status } from "../../models/enums/status.enums";

export function returnStatusUppercase(status: string): string {
    switch (status) {
        case 'completado': return Status.COMPLETADO;
        case 'en-proceso': return Status.EN_PROCESO;
        case 'pendiente': return Status.PENDIENTE;
        default: return status;
    }
}

export function returnStatusClass(status: string): string {
    switch (status) {
        case 'completado':
        case Status.COMPLETADO:
        return 'status-completado';

        case 'en-proceso':
        case Status.EN_PROCESO:
        return 'status-en-proceso';

        case 'pendiente':
        case Status.PENDIENTE:
        return 'status-pendiente';

        default:
        return 'status-default';
    }
}