export interface ValidarCamionResponse {
    ExisteCamion: boolean;
    EsCamionEscalable: boolean;
}

export const MSG_ALERTA_NO_ESCALABLE = { severity: 'warn', summary: 'Escalable', detail: 'Los datos no corresponden a un camión escalable.', life: 5000 };