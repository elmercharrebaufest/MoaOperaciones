import { Component, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-expandable-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './expandable-section.html',
  styleUrls: ['./expandable-section.scss']
})
export class ExpandableSectionComponent {
  title = input.required<string>();
  isExpanded = signal(false);
  
  toggle() {
    this.isExpanded.update(v => !v);
  }
}