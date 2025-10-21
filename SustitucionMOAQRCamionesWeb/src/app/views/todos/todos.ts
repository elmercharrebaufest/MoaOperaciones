import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DataService } from '../../infrastructure/services/data.service';
import { I18nService } from '../../infrastructure/services/i18n.service';
import { MATERIAL } from '../../shared/material';

@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [
    CommonModule,
    ...MATERIAL
  ],
  templateUrl: './todos.html',
  styleUrls: ['./todos.scss']
})
export class TodosComponent implements OnInit {
  public readonly isLoading = signal(false);
  public readonly dataService = inject(DataService);
  protected readonly i18n = inject(I18nService);

  // Computed signal for completion percentage
  readonly completionPercentage = computed(() => {
    const total = this.dataService.todos().length;
    const completed = this.dataService.completedTodos().length;
    return total > 0 ? Math.round((completed / total) * 100) : 0;
  });

  constructor() {}

  ngOnInit(): void {
    this.loadTodos();
  }

  private loadTodos(): void {
    this.isLoading.set(true);
    // Llamada a ApiService.actualizarSolp y guardar resultado en variable
    const nroSolp = '123'; // Reemplaza con el valor adecuado
    let resultadoSolp: any;
    this.dataService.apiService.actualizarSolp(nroSolp).subscribe({
      next: (response) => {
        resultadoSolp = response;
        console.log('Resultado actualizarSolp:', resultadoSolp);
      },
    });   

    this.dataService.getTodos().subscribe({
      next: (todos) => {
        this.isLoading.set(false);
        console.log('Loaded todos:', todos.length);
      },
      error: (error) => {
        this.isLoading.set(false);
        console.error('Error loading todos:', error);
      }
    });
  }

  toggleTodo(todoId: number): void {
    this.dataService.toggleTodo(todoId).subscribe({
      next: (updatedTodo) => {
        console.log('Todo updated:', updatedTodo);
      },
      error: (error) => {
        console.error('Error updating todo:', error);
      }
    });
  }
}
