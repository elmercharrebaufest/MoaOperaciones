import { Component, input } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-stage-progress',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './stage-progress.html',
  styleUrls: ['./stage-progress.scss']
})
export class StageProgressComponent {
  currentStage = input.required<number>();
  totalStages = input.required<number>();
  stageName = input.required<string>();
  stageDate = input.required<Date>();
  estimatedTime = input.required<string>();
}