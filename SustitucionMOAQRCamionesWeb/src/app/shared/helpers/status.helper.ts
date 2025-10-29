import { Status } from "../../models/enums/status.enums";

export function returnStatusUppercase(status: string, rechazado = false): string {
  if (hasTheWorkflowBeenRejected(rechazado, status)) {
    return Status.RECHAZADO;
  }

  switch (status) {
    case 'completado':
    case Status.COMPLETADO:
      return Status.COMPLETADO;

    case 'en-proceso':
    case Status.EN_PROCESO:
      return Status.EN_PROCESO;

    case 'pendiente':
    case Status.PENDIENTE:
      return Status.PENDIENTE;

    default:
      return status;
  }
}

export function returnStatusClass(status: string, rechazado = false): string {
  if (hasTheWorkflowBeenRejected(rechazado, status)) {
    return 'status-rechazado';
  }

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

function hasTheWorkflowBeenRejected(rechazado: boolean, status: string): boolean {
  return Boolean(
    rechazado === true &&
    (status === 'en-proceso' || status === Status.EN_PROCESO)
  );
}