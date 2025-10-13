import { Component, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-expandable-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './expandable-section.html',
  styleUrls: ['./expandable-section.scss']
})
export class ExpandableSectionComponent {
  title = input.required<string>();
  navigateTo = input<'carga' | 'planta' | null>(null);
  isExpanded = signal(false);

  constructor(private router: Router) {}

  toggle() {
    const navType = this.navigateTo();
    if (navType) {
      this.router.navigate(['/information-detail'], {
        queryParams: { type: navType }
      });
    } else {
      this.isExpanded.update(value => !value);
    }
  }
}