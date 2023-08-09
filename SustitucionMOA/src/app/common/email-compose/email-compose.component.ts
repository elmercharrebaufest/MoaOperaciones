import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { EmailComposeModel } from './email-compose.model';
import { EmailComposeService } from './email-compose.service';

@Component({
  selector: 'email-compose',
  templateUrl: './email-compose.component.html',
  styleUrls: ['./email-compose.component.css']
})
export class EmailComposeComponent implements OnInit {

  @Output()
  sendEmailWrapperEmitter = new EventEmitter<EmailComposeModel>(); 

  public visible: boolean = false;
  public model: EmailComposeModel = new EmailComposeModel();

  formGroupEmail: FormGroup

  constructor(private emailComposeService: EmailComposeService,
    private formBuilder: FormBuilder) {
    this.emailComposeService.toogleOn.subscribe(value => 
      {
        this.visible = value.visible; 
        this.model = value.model;
      });
  }

  ngOnInit(): void {
    this.formGroupEmail = this.formBuilder.group({    
      from: new FormControl('', Validators.required),            
      to: new FormControl('', Validators.required),
      cc: new FormControl(''),
      bcc: new FormControl(''),
      subject: new FormControl('', Validators.required),
      body: new FormControl('', Validators.required),
    });
  }

  showInputError(fieldName: string): boolean {
    if (this.formGroupEmail && this.formGroupEmail.controls) {
        return (this.formGroupEmail.controls[fieldName].invalid || (this.formGroupEmail.controls[fieldName].errors && this.formGroupEmail.controls[fieldName].errors.required));
    }

    return false;
  }

  isEmailInvalid(value: any) {
    const EMAIL_REGEXP = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    if (value != null && value !== '') {
        return EMAIL_REGEXP.test(value)
    }
  }

  validateCcEmailAddress(event: any): void {
    let value = event.value;
    if (!value) {
      return;
    }
    if (!this.isEmailInvalid(value)) {
      this.model.cc.pop(); 
    }
  }

  validateBccEmailAddress(event: any): void {
    let value = event.value;
    if (!value) {
      return;
    }
    if (!this.isEmailInvalid(value)) {
      this.model.bcc.pop(); 
    }
  }

  onSendEmail() {
    if (this.canSendEmail) {
      this.sendEmailWrapperEmitter.next(this.model);
    }
  }

  onClose() {
    this.emailComposeService.close();
  }

  public get canSendEmail(): boolean {
    let result = this.formGroupEmail.valid && this.isValidFromEmailDomain;
    return result;
  }

  public get isValidFromEmailDomain(): boolean {
    if (!this.model.from) {
      return true;
    }
    const molinosAgroDomain = "molinosagro.com.ar";
    const fromEmaildomain = this.model.from.substring(this.model.from.lastIndexOf("@") +1);
    return molinosAgroDomain == fromEmaildomain.toLocaleLowerCase();
  }

}
