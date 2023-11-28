export interface ValidarCamionResponse {
    ExisteCamion: boolean;
    EsCamionEscalable: boolean;
}

export const MSG_ALERTA_NO_ESCALABLE = { severity: 'warn', summary: 'Escalable', detail: 'Los datos no corresponden a un camión escalable.', life: 5000 };
export const MSG_ALERTA_CAMION_NO_EXISTE = { severity: 'warn', summary: 'Camión', detail: 'Las patentes no corresponden a un camión válido.', life: 5000 };