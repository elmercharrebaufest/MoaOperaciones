import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private readonly _isLoading = signal(false);
  private readonly _loadingMessage = signal('Cargando...');
  private readonly _loadingCount = signal(0);

  // Readonly signals para exponer el estado
  readonly isLoading = this._isLoading.asReadonly();
  readonly loadingMessage = this._loadingMessage.asReadonly();
  readonly loadingCount = this._loadingCount.asReadonly();

  setLoading(loading: boolean, message: string = 'Cargando...'): void {
    if (loading) {
      this.increment();
    } else {
      this.decrement();
    }
    this._loadingMessage.set(message);
  }

  increment(): void {
    const newCount = this._loadingCount() + 1;
    this._loadingCount.set(newCount);
    this._isLoading.set(newCount > 0);
  }

  decrement(): void {
    const newCount = Math.max(0, this._loadingCount() - 1);
    this._loadingCount.set(newCount);
    this._isLoading.set(newCount > 0);
  }

  reset(): void {
    this._loadingCount.set(0);
    this._isLoading.set(false);
  }

  showLoading(message: string = 'Cargando...'): void {
    this.setLoading(true, message);
  }

  hideLoading(): void {
    this.setLoading(false);
  }
}
