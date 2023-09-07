export class EmailComposeModel {
    from: string;
    to: Array<string> = new Array<string>();
    cc: Array<string> = new Array<string>();
    bcc: Array<string> = new Array<string>();
    subject: string;
    body: string;
    downloadLinkUrl: string;
}

export interface EmailCompose {
    visible: boolean;
    model: EmailComposeModel;
}