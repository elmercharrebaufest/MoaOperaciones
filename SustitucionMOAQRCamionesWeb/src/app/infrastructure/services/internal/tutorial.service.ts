import { Injectable, signal } from "@angular/core"
import { CookieService } from "./cookie.service"

export interface TutorialStep {
  title: string
  description: string
  stepNumber: number
  totalSteps: number
  targetElement: string
  position: "top" | "bottom" | "left" | "right"
}

@Injectable({
  providedIn: "root",
})
export class TutorialService {
  private readonly TUTORIAL_COOKIE_NAME = "moa_tutorial_completed"

  showWelcomeModal = signal(false)
  showTutorial = signal(false)
  currentStep = signal(0)

  tutorialSteps: TutorialStep[] = [
    {
      title: "Datos del vehículo en seguimiento",
      description: "Acá se muestra la patente ingresada y la información correspondiente a ese camión.",
      stepNumber: 1,
      totalSteps:  4,
      targetElement:  ".shipment-card-tutorial-target",
      position: "bottom",
    },
    {
      title: "Actualizar información",
      description: "Tocá este botón cuando quieras ver los últimos datos disponibles.",
      stepNumber: 2,
      totalSteps: 4,
      targetElement: ".actualizar-button",
      position:  "bottom",
    },
    {
      title: "Segui el avance",
      description:
        "Arriba vas a ver en qué etapa estás. Podés desplegar para conocer las próximas etapas y ver cuáles ya se completaron.",
      stepNumber: 3,
      totalSteps: 4,
      targetElement: ".stage-section",
      position: "bottom",
    },
    {
      title:  "Información y documentación",
      description: "Desde este menú podés consultar la información general de la carga y descargar documentación.",
      stepNumber: 4,
      totalSteps: 4,
      targetElement: ".information-button",
      position: "top",
    },
  ]

  constructor(private cookieService: CookieService) {}

  shouldShowTutorial(): boolean {
    const completed = this.cookieService.getCookie(this.TUTORIAL_COOKIE_NAME)
    return completed !== "true"
  }

  startTutorial(): void {
    this.showWelcomeModal.set(false)
    this.showTutorial. set(true)
    this.currentStep.set(0)
  }

  skipTutorial(): void {
    this.showWelcomeModal.set(false)
    this.showTutorial.set(false)
    this.markTutorialComplete()
  }

  nextStep(): void {
    if (this.currentStep() < this.tutorialSteps.length - 1) {
      this.currentStep.update((step) => step + 1)
    } else {
      this.completeTutorial()
    }
  }

  completeTutorial(): void {
    this.showTutorial.set(false)
    this.markTutorialComplete()
  }

  private markTutorialComplete(): void {
    this.cookieService.setCookie(this.TUTORIAL_COOKIE_NAME, "true", 365)
  }

  initializeTutorial(): void {
    if (this.shouldShowTutorial()) {
      this.showWelcomeModal.set(true)
    }
  }

  getCurrentStep(): TutorialStep | null {
    const step = this.currentStep()
    return step < this.tutorialSteps.length ? this.tutorialSteps[step] : null
  }
}