import { Component, OnInit, ViewChild } from '@angular/core';
import { CmiOption, ScormService, storageChangeObservable } from '../scorm.service';
import { ActualizarProgresoReqDto, CURSOS_BASE_PATH, CursoUsuarioDto, EstadoCurso } from '../../common/models/cursos/Curso';
import { CursosBaseComponent } from '../curso-base.component';
import { CursosService } from '../cursos.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { BehaviorSubject, Subscription, fromEvent } from 'rxjs';
import { debounceTime, finalize } from 'rxjs/operators';

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
    CmiOption.SuspendData,
  ]
  get isVisible(): boolean {
    return this.cursosDisponibles && !!this.cursosDisponibles.length
  }

  cursoVerProgreso?: BehaviorSubject<CursoUsuarioDto | null> = new BehaviorSubject(null);
  cursoCursando?: BehaviorSubject<CursoUsuarioDto | null> = new BehaviorSubject(null);
  cursosDisponibles: CursoUsuarioDto[] = []

  constructor(
    private scormService: ScormService,
    service: CursosService,
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

  subscriptionCambiosProgresoCurso?: Subscription;
  subscriptionGuardadoProgreso?: Subscription;

  abrirCurso(curso: CursoUsuarioDto) {
    this.spinnerComponent.showIt();
    this.service.progreso(curso.CursoId)
      .pipe(finalize(() => this.spinnerComponent.hideIt()))
      .subscribe(res => {
        const progreso = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
        if (typeof progreso === "string") {
          this.cursoCursando.next(curso);
          localStorage.setItem(CmiOption.SuspendData, progreso);
          setTimeout(() => {
            const windowCurso = this.scormService.inicializarCurso(curso.AccesoCurso)
            fromEvent(
              windowCurso,
              'beforeunload'
            ).pipe(debounceTime(100)).subscribe(() => this.guardarProgreso())
            this.subscriptionCambiosProgresoCurso = this.cambiosProgreso$
              .subscribe(() => this.guardarProgreso())
          }, 0)
        }
      })
  }
  setTabs(): void {
    this.setMenuSeccionTab(CURSOS_BASE_PATH, 'Mis Cursos');
  }
  verProgreso(curso: CursoUsuarioDto) {
    this.cursoVerProgreso.next(curso)
  }

  guardarProgreso() {
    this.spinnerComponent.showIt()
    if (this.subscriptionGuardadoProgreso) {
      this.subscriptionGuardadoProgreso.unsubscribe()
    }
    this.subscriptionGuardadoProgreso = this.service.actualizarProgreso(this.progresoActual())
      .pipe(finalize(() => this.spinnerComponent.hideIt()))
      .subscribe(res => {
        const guardado = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
        if (!guardado) {

        }
      }
      );
  }
  progresoActual(nuevoEstado?: EstadoCurso): ActualizarProgresoReqDto {
    const estado = localStorage.getItem(CmiOption.LessonStatus)
    console.log(estado)
    return {
      CursoId: this.cursoCursando.value.CursoId,
      DatosProgreso: localStorage.getItem(CmiOption.SuspendData),
      TiempoSesion: localStorage.getItem(CmiOption.SessionTime),
      NuevoEstado: nuevoEstado ? nuevoEstado :
        estado == "complete" ? EstadoCurso.Completado :
          EstadoCurso.EnProgreso,
    }
  }
}
