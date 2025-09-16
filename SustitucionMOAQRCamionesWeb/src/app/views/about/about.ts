import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UiCard } from '../../shared/ui-card/ui-card';
import { MATERIAL } from '../../shared/material';

@Component({
  standalone: true,
  selector: 'app-about',
  imports: [CommonModule, UiCard, ...MATERIAL],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './about.html',
  styleUrl: './about.scss',
})
export class About {

  constructor() { }

}
