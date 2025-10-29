import { Component, input, output, signal } from '@angular/core';
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
  isExpanded = input.required<boolean>();
  
  stageChanged = output<number>();
  toggleExpanded = output<void>();

  getSteps() {
    return Array.from({ length: this.totalSteps() }, (_, i) => i);
  }

  onStepClick(step: number) {
    this.stageChanged.emit(step);
  }

  onToggleExpanded() {
    this.toggleExpanded.emit();
  }
}