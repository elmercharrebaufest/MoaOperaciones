import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL } from '../material';

@Component({
  selector: 'app-ui-card',
  standalone: true,
  imports: [CommonModule, ...MATERIAL],
  templateUrl: './ui-card.html',
  styleUrl: './ui-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UiCard {
  @Input() title = 'Card';
  @Input() subtitle = '';
}
