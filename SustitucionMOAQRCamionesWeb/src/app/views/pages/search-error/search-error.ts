import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-search-error',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './search-error.html',
  styleUrls: ['./search-error.scss']
})
export class SearchErrorComponent implements OnInit {
  mensaje = signal('No encontramos resultados con las credenciales ingresadas');

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['mensaje']) {
        this.mensaje.set(params['mensaje']);
      }
    });
  }

  onRetry() {
    this.router.navigate(['/login']);
  }
}