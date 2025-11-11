import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-section-wrapper',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './section-wrapper.html',
  styleUrls: ['./section-wrapper.scss']
})
export class SectionWrapperComponent {
  backgroundColor = input<string>('#f5f5f5');
  padding = input<string>('4px');
  roundedBottom = input<boolean>(false);
}