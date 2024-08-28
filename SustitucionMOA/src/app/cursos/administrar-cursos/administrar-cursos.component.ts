import { Component, OnInit, ViewChild } from '@angular/core';
import { CursosBaseComponent } from '../curso-base.component';
import { CursosService } from '../cursos.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { CURSOS_BASE_PATH, CursoDto } from '../../common/models/cursos/Curso';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { BehaviorSubject } from 'rxjs';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-administrar-cursos',
  templateUrl: './administrar-cursos.component.html',
  styleUrls: ['./administrar-cursos.component.css']
})
export class AdministrarCursosComponent extends CursosBaseComponent implements OnInit {
  @ViewChild("mensaje")
  mensajeComponent: MensajeComponent;

  cursoAAsignarAlumnos?: BehaviorSubject<CursoDto | null> = new BehaviorSubject(null);
  cursoAVerProgreso?: BehaviorSubject<CursoDto | null> = new BehaviorSubject(null);
  get isVisible(): boolean {
    return this.cursos && !!this.cursos.length
  }
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
    this.spinnerComponent.showIt()
    this.service.disponibles()
      .pipe(finalize(() => this.spinnerComponent.hideIt()))
      .subscribe((res) => {
        const cursos = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
        if (cursos)
          this.cursos = cursos;
      })
  }
  setTabs(): void {
    this.setMenuSeccionTab(CURSOS_BASE_PATH, 'Administrar Cursos');
  }

  asignarAlumnos(curso: CursoDto) {
    this.cursoAAsignarAlumnos.next(curso)
  }
  verProgresoAlumnos(curso: CursoDto) {
    this.cursoAVerProgreso.next(curso)
  }
}
