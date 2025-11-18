import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-container',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './container.html',
  styleUrls: ['./container.scss']
})
export class ContainerComponent {
  padding = input<string>('p-4 md:p-6');
  shadow = input<string>('shadow-md');
  rounded = input<string>('rounded-xl');
  background = input<string>('bg-white');
  maxWidth = input<string>('max-w-3xl');
  gap = input<string>('gap-2');
  marginBottom = input<string>('');
}