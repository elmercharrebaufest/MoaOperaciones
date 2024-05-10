import { Component, OnInit, ViewChild } from '@angular/core';
import { CursosBaseComponent } from '../curso-base.component';
import { CursosService } from '../cursos.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { CursoDto } from '../../common/models/cursos/Curso';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';

@Component({
  selector: 'app-administrar-cursos',
  templateUrl: './administrar-cursos.component.html',
  styleUrls: ['./administrar-cursos.component.css']
})
export class AdministrarCursosComponent extends CursosBaseComponent implements OnInit {
  @ViewChild("mensaje")
  mensajeComponent: MensajeComponent;

  cursos: CursoDto[] = [];
  constructor(service: CursosService,
    navService: NavService,
    sessionDataService: SessionDataService,
    securityService: SecurityService,
    floatMsgService: FloatMsgService,
    modalService: ModalService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService)
  }

  extraOnInit() {
    this.service.disponibles().subscribe((res) => {
      const cursos = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
      if (cursos)
        this.cursos = cursos;
    })
  }

}
