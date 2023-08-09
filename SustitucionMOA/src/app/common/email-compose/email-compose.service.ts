import { Injectable, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { EmailCompose, EmailComposeModel } from './email-compose.model';

@Injectable({
  providedIn: 'root',
})
export class EmailComposeService implements OnDestroy {
  public toogleOn = new Subject<EmailCompose>();

  private visible = false;

  ngOnDestroy() {
    if (this.visible) { 
      this.toogleOn.unsubscribe();
    }
  }

  show(emailModel: EmailComposeModel) {
    this.visible = true;
    this.toogleOn.next(
      {
        visible: this.visible, 
        model: emailModel
      } as EmailCompose
    );
  }

  close() {
    this.visible = false;
    this.toogleOn.next(
      {
        visible: this.visible, 
        model: new EmailComposeModel()
      } as EmailCompose
    );
  }
}
