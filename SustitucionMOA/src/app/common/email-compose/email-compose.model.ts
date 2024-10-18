export interface EmailInfo {
    Id: string;
    CodigoDescripcion: string;
}
export class EmailComposeModel<T> {
    from: string;
    to: Array<T> = new Array<T>();
    cc: Array<string> = new Array<string>();
    bcc: Array<string> = new Array<string>();
    subject: string;
    body: string;
    downloadLinkUrl: string;
    tieneAdjuntos: boolean;
}


export interface EmailCompose <T>{
    visible: boolean;
    model: EmailComposeModel<T>;
}