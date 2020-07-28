import { Component } from '@angular/core';
import { SecurityService } from '../../common/services/SecurityService';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ModalService } from '../../common/services/ModalService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { BaseService } from '../../common/services/BaseService';
import { BaseComponent } from '../../common/base-components/base-component';
import { EmpresaGranosService } from './empresa-granos.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-empresa-granos',
  templateUrl: './app/alta-proveedores/empresa-granos/empresa-granos.component.html',
  styleUrls: ['./app/alta-proveedores/empresa-granos/empresa-granos.component.css', '../Content/css/bootstrap.min.css'],
  providers: [EmpresaGranosService]
})
export class EmpresaGranosComponent extends BaseComponent {

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  constructor(protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securytiService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
    super(navService, securytiService, floatMsgService, modalService);
  }

  ngOnInit() {
    this.setTabs();
    this.navService.setSeccionList([]);
    this.firstFormGroup = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email])
    });
    this.secondFormGroup = new FormGroup({
      password: new FormControl('', Validators.required)
    });
  }

  get email() {
    return this.firstFormGroup.get('email');
  }
  get password() {
    return this.secondFormGroup.get('password');
  }

  onSubmit() {
    // do something here
  }
}
