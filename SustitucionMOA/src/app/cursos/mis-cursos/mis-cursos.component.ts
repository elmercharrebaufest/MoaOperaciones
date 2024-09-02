import { Component, OnInit, ViewChild } from '@angular/core';
import { CmiOption, ScormService, storageChangeObservable } from '../scorm.service';
import { ActualizarProgresoReqDto, CURSOS_BASE_PATH, CursoUsuarioDto, EstadoCurso, ProgresoResDto } from '../../common/models/cursos/Curso';
import { CursosBaseComponent } from '../curso-base.component';
import { CursosService } from '../cursos.service';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { BehaviorSubject, Observable, Subject, Subscription, fromEvent } from 'rxjs';
import { debounceTime, finalize } from 'rxjs/operators';
import { ApiResponse } from '../../common/models/response';

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

  localSubscriptions = new Subscription();

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
    this.localSubscriptions.add(
      this.guardadoAutomatico$.pipe(debounceTime(500)).subscribe(
        () => this.guardadoAutomatico()
      )
    )
  }
  setTabs(): void {
    this.setMenuSeccionTab(CURSOS_BASE_PATH, 'Mis Cursos');
  }
  verProgreso(curso: CursoUsuarioDto) {
    this.cursoVerProgreso.next(curso)
  }

  subscriptionCambiosProgresoCurso?: Subscription;
  subscriptionGuardadoProgreso?: Subscription;

  abrirCurso(curso: CursoUsuarioDto) {
    this.spinnerComponent.showIt();
    this.service.progreso(curso.CursoId)
      .pipe(finalize(() => this.spinnerComponent.hideIt()))
      .subscribe(res => {
        const progreso = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
        if (progreso) {
          this.cursoCursando.next(curso);
          if (!progreso.DetalleProgreso) {
            this.iniciarCurso(curso, progreso)
          } else {
            this.continuarCurso(curso, progreso)
          }
        }
      })
  }
  iniciarCurso(curso: CursoUsuarioDto, progreso: ProgresoResDto) {
    this.subscriptionGuardadoProgreso = this.guardarProgreso(EstadoCurso.Iniciado)
      .subscribe(res => {
        const guardadoCorrecto = this.handlerGuardado(res);
        if (guardadoCorrecto) {
          this.continuarCurso(curso, progreso)
        }
      })
  }

  continuarCurso(curso: CursoUsuarioDto, progreso: ProgresoResDto) {
    localStorage.clear()
    localStorage.setItem(CmiOption.SuspendData, progreso.DetalleProgreso);
    setTimeout(() => {
      const windowCurso = this.scormService.inicializarCurso(curso.AccesoCurso)
      fromEvent(
        windowCurso,
        'beforeunload'
      ).pipe(debounceTime(50)).subscribe(() => this.guardadoAutomatico$.next())
      this.subscriptionCambiosProgresoCurso = this.cambiosProgreso$
        .subscribe(() => this.guardadoAutomatico$.next())
    }, 50)
  }

  guardadoAutomatico$ = new Subject<void>();

  guardadoAutomatico() {
    this.subscriptionGuardadoProgreso = this.guardarProgreso()
      .subscribe(res => this.handlerGuardado(res))
  }

  guardarProgreso(nuevoEstado?: EstadoCurso): Observable<ApiResponse<boolean>> {
    this.spinnerComponent.showIt()
    if (this.subscriptionGuardadoProgreso) {
      this.subscriptionGuardadoProgreso.unsubscribe()
    }
    return this.service.actualizarProgreso(this.progresoActual(nuevoEstado))
      .pipe(finalize(() => {
        this.spinnerComponent.hideIt()
      }))
  }
  handlerGuardado(res: ApiResponse<boolean>): boolean {
    const guardado = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
    if (!guardado) {

    }
    return guardado
  }

  progresoActual(nuevoEstado?: EstadoCurso): ActualizarProgresoReqDto {
    const estado = localStorage.getItem(CmiOption.LessonStatus)
    const tiempo = localStorage.getItem(CmiOption.SessionTime)
    if (tiempo) {
      localStorage.setItem(CmiOption.SessionTime, "0000:00:00.00")
    }
    return {
      CursoId: this.cursoCursando.value.CursoId,
      DatosProgreso: localStorage.getItem(CmiOption.SuspendData),
      TiempoSesion: tiempo,
      NuevoEstado: nuevoEstado ? nuevoEstado :
        estado == "completed" ? EstadoCurso.Completado :
          EstadoCurso.EnProgreso,
    }
  }
}
