import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { formatDate } from '../../shared/helpers/date.helper';

@Component({
  selector: 'app-stage-progress',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stage-progress.html',
  styleUrls: ['./stage-progress.scss']
})
export class StageProgressComponent {
  currentStage = input.required<number>();
  totalStages = input.required<number>();
  stageName = input.required<string>();
  stageDate = input.required<Date>();
  estimatedTime = input.required<string>();

  formatStageDate(): string {
    return formatDate(this.stageDate());
  }
}