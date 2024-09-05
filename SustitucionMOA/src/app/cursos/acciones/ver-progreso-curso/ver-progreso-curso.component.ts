import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CursoUsuarioDto, ProgresoAlumnoEnCursoDto } from '../../../common/models/cursos/Curso';
import { CursosService } from '../../cursos.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { BaseComponent } from '../../../common/base-components/base-component';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SecurityService } from '../../../common/services/SecurityService';
import { NavService } from '../../../common/services/NavService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { MensajeComponent } from '../../../common/view-child/mensaje/mensaje.component';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-ver-progreso-curso',
  templateUrl: './ver-progreso-curso.component.html',
  styleUrls: ['./ver-progreso-curso.component.css']
})
export class VerProgresoCursoComponent extends BaseComponent implements OnInit, OnDestroy {
  @ViewChild("mensaje")
  mensajeComponent: MensajeComponent;
  @ViewChild("spinner")
  spinnerComponent: SpinnerComponent;
  subscriptionsLocal = new Subscription();
  @Input()
  curso?: BehaviorSubject<CursoUsuarioDto | null>;
  display = false;

  constructor(private service: CursosService,
    protected sessionDataService: SessionDataService,
    navService: NavService, securityService: SecurityService,
    floatMsgService: FloatMsgService, modalService: ModalService
  ) {
    super(navService, securityService, floatMsgService, modalService)
  }
  ngOnInit() {
    this.subscriptions.add(
      this.curso.subscribe(curso => {
        this.display = !!curso;
        if (this.display) {
          this.buscarProgreso(curso)
        }
      })
    )
  }

  progresoEnCurso: ProgresoAlumnoEnCursoDto = null;

  buscarProgreso(curso: CursoUsuarioDto) {
    this.spinnerComponent.showIt();
    this.service.obtenerProgresoAlumno(curso.CursoId)
      .pipe(finalize(() => this.spinnerComponent.hideIt()))
      .subscribe(res => {
        const progreso = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
        this.progresoEnCurso = progreso;
      })

  }

  cerrar() {
    this.curso.next(null)
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe()
  }

}
