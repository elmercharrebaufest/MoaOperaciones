import { Injectable, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { EmailCompose, EmailComposeModel, EmailInfo } from './email-compose.model';

@Injectable({
  providedIn: 'root',
})
export class EmailComposeService implements OnDestroy {
  public toogleOn = new Subject<EmailCompose<EmailInfo>>();

  private visible = false;

  ngOnDestroy() {
    if (this.visible) {
      this.toogleOn.unsubscribe();
    }
  }

  show(emailModel: EmailComposeModel<EmailInfo>) {
    this.visible = true;
    this.toogleOn.next(
      {
        visible: this.visible,
        model: emailModel
      } as EmailCompose<EmailInfo>
    );
  }

  close() {
    this.visible = false;
    this.toogleOn.next(
      {
        visible: this.visible,
        model: new EmailComposeModel()
      } as EmailCompose<EmailInfo>
    );
  }
}
