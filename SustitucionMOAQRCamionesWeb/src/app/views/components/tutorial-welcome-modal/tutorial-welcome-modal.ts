import { Component, output } from "@angular/core"
import { CommonModule } from "@angular/common"

@Component({
  selector: "app-tutorial-welcome-modal",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./tutorial-welcome-modal.html",
  styleUrls: ["./tutorial-welcome-modal.scss"],
})
export class TutorialWelcomeModalComponent {
  startTutorial = output<void>()
  skipTutorial = output<void>()

  onStartTutorial(): void {
    this.startTutorial.emit()
  }

  onSkipTutorial(): void {
    this.skipTutorial.emit()
  }
}
