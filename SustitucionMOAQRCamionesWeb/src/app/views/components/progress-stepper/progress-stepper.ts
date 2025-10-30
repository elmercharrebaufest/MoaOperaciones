import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Etapa } from '../../../models/estado-etapas.model';
import { returnStepperButtonClass } from '../../../shared/helpers/status.helper';

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
  stages = input.required<Etapa[]>();
  rechazado = input.required<boolean>();
  
  stageChanged = output<number>();
  toggleExpanded = output<void>();

  getSteps() {
    return Array.from({ length: this.totalSteps() }, (_, i) => i);
  }

  getIconPath(stageName: string): string {
    const normalizedName = stageName.toLowerCase().replace(/[^a-z0-9]/g, '_');
    return `assets/etapas/${normalizedName}_icon.svg`;
  }

  getButtonClass(step: number): string {
    if (step === this.currentStep()) {
      return returnStepperButtonClass(
        this.stages()[step].estado,
        this.rechazado()
      );
    }
    return 'stepper-button-default';
  }

  onToggleExpanded() {
    this.toggleExpanded.emit();
  }
}