import { Component, input, output, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-progress-stepper',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './progress-stepper.html',
  styleUrls: ['./progress-stepper.scss']
})
export class ProgressStepperComponent {
  totalSteps = input.required<number>();
  currentStep = input.required<number>();
  stageChanged = output<number>();
  
  isExpanded = signal(false);
  
  steps = computed(() => Array.from({ length: this.totalSteps() }, (_, i) => i + 1));
  
  goToPrevious() {
    if (this.currentStep() > 1) {
      this.stageChanged.emit(this.currentStep() - 2); // -2 because index is 0-based
    }
  }
  
  goToNext() {
    if (this.currentStep() < this.totalSteps()) {
      this.stageChanged.emit(this.currentStep()); // currentStep is already the next index
    }
  }
  
  toggleExpanded() {
    this.isExpanded.update(v => !v);
  }
}