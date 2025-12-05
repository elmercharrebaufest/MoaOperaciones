import {
  Component,
  input,
  output,
  computed,
  effect,
  ElementRef,
  Renderer2,
  ViewChild,
  OnDestroy,
  HostListener,
  signal,
} from "@angular/core"
import { CommonModule } from "@angular/common"
import type { TutorialStep } from "../../../infrastructure/services/internal/tutorial.service"

@Component({
  selector: "app-tutorial-tooltip",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./tutorial-tooltip.html",
  styleUrls: ["./tutorial-tooltip.scss"],
})
export class TutorialTooltipComponent implements OnDestroy {
  step = input.required<TutorialStep>()
  nextStep = output<void>()

  @ViewChild("tooltipContainer") tooltipContainer!: ElementRef

  tooltipPosition = signal({ top: 0, left: 0 })
  arrowPosition = signal(0)
  
  private currentTargetElement: HTMLElement | null = null;
  private resizeObserver: ResizeObserver | null = null;

  isLastStep = computed(() => {
    const currentStep = this.step()
    return currentStep.stepNumber === currentStep.totalSteps
  })

  constructor(private renderer: Renderer2) {
    effect(() => {
      const currentStep = this.step()
      setTimeout(() => {
        this.highlightAndPosition(currentStep)
      }, 100)
    })
  }

  @HostListener('window:resize')
  onResize() {
    if (this.step()) {
      this.highlightAndPosition(this.step());
    }
  }

  onNext(): void {
    this.cleanUpPreviousTarget();
    this.nextStep.emit()
  }

  ngOnDestroy() {
    this.cleanUpPreviousTarget();
    if (this.resizeObserver) {
      this.resizeObserver.disconnect();
    }
  }

  private highlightAndPosition(step: TutorialStep): void {
    this.cleanUpPreviousTarget();

    const target = document.querySelector(step.targetElement) as HTMLElement;
    if (!target) return;

    this.currentTargetElement = target;

    target.scrollIntoView({ behavior: "smooth", block: "center" });

    // Use the class defined in tracking.scss to handle background color and shadows
    this.renderer.addClass(target, 'tutorial-highlight');
    
    // Ensure styles that might not be in the class are applied for the overlay effect
    const computedStyle = window.getComputedStyle(target);
    if (computedStyle.position === 'static') {
      this.renderer.setStyle(target, 'position', 'relative');
    }
    this.renderer.setStyle(target, 'pointer-events', 'none'); 
    
    this.calculateCoords(target, step.position);
  }

  private calculateCoords(target: HTMLElement, position: string) {
    if (!this.tooltipContainer) return;

    const targetRect = target.getBoundingClientRect();
    const tooltipRect = this.tooltipContainer.nativeElement.getBoundingClientRect();
    const gap = 15;
    
    let top = 0;
    let left = 0;

    // Mobile detection
    const isMobile = window.innerWidth < 768;

    if (isMobile) {
      // Center on screen for mobile
      left = (window.innerWidth - tooltipRect.width) / 2;
    } else {
      // Center relative to target for desktop
      left = targetRect.left + (targetRect.width / 2) - (tooltipRect.width / 2);
      
      // Clamp to screen edges
      const padding = 10;
      if (left < padding) left = padding;
      if (left + tooltipRect.width > window.innerWidth - padding) {
        left = window.innerWidth - tooltipRect.width - padding;
      }
    }

    // Vertical positioning
    if (position === 'top') {
      top = targetRect.top - tooltipRect.height - gap;
    } else {
      top = targetRect.bottom + gap;
    }

    // Calculate Arrow Position relative to the tooltip box
    const targetCenter = targetRect.left + (targetRect.width / 2);
    let arrowLeft = targetCenter - left;

    // Clamp arrow to keep it inside the tooltip (accounting for border radius)
    const arrowPadding = 12;
    if (arrowLeft < arrowPadding) arrowLeft = arrowPadding;
    if (arrowLeft > tooltipRect.width - arrowPadding) arrowLeft = tooltipRect.width - arrowPadding;

    this.tooltipPosition.set({ top, left });
    this.arrowPosition.set(arrowLeft);
  }

  private cleanUpPreviousTarget() {
    if (this.currentTargetElement) {
      this.renderer.removeClass(this.currentTargetElement, 'tutorial-highlight');
      this.renderer.removeStyle(this.currentTargetElement, 'position');
      this.renderer.removeStyle(this.currentTargetElement, 'pointer-events');
      this.currentTargetElement = null;
    }
  }
}