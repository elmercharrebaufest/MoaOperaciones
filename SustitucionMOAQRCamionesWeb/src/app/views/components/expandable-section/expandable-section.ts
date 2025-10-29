import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-expandable-section',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './expandable-section.html',
  styleUrls: ['./expandable-section.scss']
})
export class ExpandableSectionComponent {
  title = input.required<string>();
  navigateTo = input<'carga' | 'planta' | null>(null);

  constructor(private router: Router) {}

  onClick() {
    const navType = this.navigateTo();
    if (navType) {
      this.router.navigate(['/information'], {
        queryParams: { type: navType }
      });
    }
  }
}