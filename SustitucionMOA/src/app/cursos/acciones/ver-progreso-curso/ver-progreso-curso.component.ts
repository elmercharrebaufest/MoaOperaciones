import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { CursoUsuarioDto } from '../../../common/models/cursos/Curso';
import { CursosService } from '../../cursos.service';
import { BehaviorSubject, Subscription } from 'rxjs';

@Component({
  selector: 'app-ver-progreso-curso',
  templateUrl: './ver-progreso-curso.component.html',
  styleUrls: ['./ver-progreso-curso.component.css']
})
export class VerProgresoCursoComponent implements OnInit, OnDestroy {
  subscriptions = new Subscription();
  @Input()
  curso?: BehaviorSubject<CursoUsuarioDto | null>;
  display = false;
  constructor(private service: CursosService) { }

  ngOnInit() {
    this.subscriptions.add(
      this.curso.subscribe(curso => {
        this.display = !!curso;
      })
    )
  }

  cerrar() {
    this.curso.next(null)
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe()
  }

}
