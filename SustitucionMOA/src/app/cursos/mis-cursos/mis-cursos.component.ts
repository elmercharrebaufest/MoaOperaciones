import { Component, OnInit, ViewChild } from '@angular/core';
import { CmiOption, storageChangeObservable } from '../scorm.service';
import { CursoUsuarioDto } from '../../common/models/cursos/Curso';
import { CursosBaseComponent } from '../curso-base.component';
import { CursosService } from '../cursos.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';

@Component({
  selector: 'app-mis-cursos',
  templateUrl: './mis-cursos.component.html',
  styleUrls: ['./mis-cursos.component.css']
})
export class MisCursosComponent extends CursosBaseComponent implements OnInit {
  @ViewChild("mensaje")
  mensajeComponent: MensajeComponent;
  cambiosAObservar = [
    CmiOption.Completion,
    CmiOption.SessionTime,
    CmiOption.SuspendData,
  ]

  cursosDisponibles: CursoUsuarioDto[] = []

  constructor(service: CursosService,
    navService: NavService,
    sessionDataService: SessionDataService,
    securityService: SecurityService,
    floatMsgService: FloatMsgService,
    modalService: ModalService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService)
  }

  cambiosProgreso$ = storageChangeObservable(localStorage,
    (ev: StorageEvent) => this.cambiosAObservar.includes(ev.key as CmiOption));

  extraOnInit() {
    this.service.asignadosAUsuario().subscribe((res) => {
      const cursos = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
      if (cursos)
        this.cursosDisponibles = cursos
    })
  }

  abrirCurso(curso: CursoUsuarioDto) {

  }
}
