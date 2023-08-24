import { Component, ContentChild, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { fadeInAnimation } from '../../animations/fade-in.animation';
import { BehaviorSubject, Subscription } from 'rxjs';

@Component({
  selector: 'app-dropdown-input',
  templateUrl: './dropdown-input.component.html',
  styleUrls: ['./dropdown-input.component.css'],
  animations: [fadeInAnimation]
})
export class DropdownInputComponent<T> implements OnInit, OnDestroy {
  @Input() options?: Array<T> = [];
  @Input() filterKey: keyof T = "filter" as keyof T;
  @Input() valueKey?: keyof T;
  @Input() placeholder: string = "Seleccione ..";
  @Input() disabled = false;
  filteredOptions: Array<T> = this.options;
  @Input() selectedOption?: T;

  showDropdown = new BehaviorSubject(false);
  subscriptions = new Subscription();
  @ContentChild(TemplateRef) optionTemplate?: TemplateRef<any>;
  @ViewChild('filter') filterInput: ElementRef<HTMLInputElement>;
  @Output() valueChange: EventEmitter<T> = new EventEmitter();

  /** Propiedad para evitar el click outside que cierra al abrir. Requiere mejora*/
  counter = -1;

  ngOnInit(): void {
    this.subscriptions.add(
      this.showDropdown.subscribe(showDropdown => {
        if (showDropdown) {
          this.counter = -1;
          this.filteredOptions = this.options;
          setTimeout(() => this.filterInput.nativeElement.focus(), 0);
        } else
          this.counter++;
      })
    )
  }

  onInputChange(value: string) {
    this.filteredOptions = this.options.filter(option => {
      if (typeof (option) === 'string')
        return option.toLowerCase().includes(value.toLowerCase());
      if (typeof (option) === 'object') {
        const labeled = option[this.filterKey]
        if (typeof (labeled) === 'string')
          return labeled.toLowerCase().includes(value.toLowerCase())
      }
    }
    );
  }

  onSelectOption(option: T) {
    if (this.disabled)
      return;
    this.selectedOption = option;
    this.filteredOptions = [];
    this.showDropdown.next(false);
    this.valueChange.emit(this.selectedOption)
  }

  toggleDropdown() {
    if (!this.disabled)
      this.showDropdown.next(!this.showDropdown.value)
  }

  closeDropdown() {
    this.showDropdown.next(false);
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe()
  }
}
