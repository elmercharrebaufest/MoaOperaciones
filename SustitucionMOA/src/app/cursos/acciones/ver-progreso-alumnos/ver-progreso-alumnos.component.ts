import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MensajeComponent } from '../../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { BehaviorSubject, Subscription } from 'rxjs';
import { CursoDto, ProgresoAlumnoEnCursoDto } from '../../../common/models/cursos/Curso';
import { BaseComponent } from '../../../common/base-components/base-component';
import { CursosService } from '../../cursos.service';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SecurityService } from '../../../common/services/SecurityService';
import { NavService } from '../../../common/services/NavService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-ver-progreso-alumnos',
  templateUrl: './ver-progreso-alumnos.component.html',
  styleUrls: ['./ver-progreso-alumnos.component.css']
})
export class VerProgresoAlumnosComponent extends BaseComponent implements OnInit {
  @ViewChild("mensaje")
  mensajeComponent: MensajeComponent;
  @ViewChild("spinner")
  spinnerComponent: SpinnerComponent;
  @Input()
  curso?: BehaviorSubject<CursoDto | null>;
  display = false;

  progresos: ProgresoAlumnoEnCursoDto[] = [];
  subscriptionsLocal = new Subscription();

  constructor(private service: CursosService,
    protected sessionDataService: SessionDataService,
    navService: NavService, securityService: SecurityService,
    floatMsgService: FloatMsgService, modalService: ModalService
  ) {
    super(navService, securityService, floatMsgService, modalService)
  }
  ngOnInit() {
    this.subscriptionsLocal.add(
      this.curso.subscribe(value => {
        this.display = !!value;
        if (this.display) {
          this.buscarProgresos(value)
        }
      })
    )
  }

  buscarProgresos(curso: CursoDto) {
    this.spinnerComponent.showIt()
    this.service.obtenerProgresoAlumnos(curso.Id)
      .pipe(finalize(() => this.spinnerComponent.hideIt()))
      .subscribe(res => {
        const progresos = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent);
        if (progresos) {
          this.progresos = progresos;
          setTimeout(() => {
            window.dispatchEvent(new Event("resize"));
          }, 0)
        }
      })
  }

  cerrar() {
    this.curso.next(null)
  }
  extraOnDestroy(): void {
    this.subscriptionsLocal.unsubscribe()
  }
}
