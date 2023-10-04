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

  showDropdown = new BehaviorSubject(false);
  subscriptions = new Subscription();
  @ContentChild(TemplateRef) optionTemplate?: TemplateRef<any>;
  @ViewChild('filter') filterInput: ElementRef<HTMLInputElement>;

  selectedValue?: T;

  @Input()
  get selected() {
    return this.selectedValue;
  }
  @Output() selectedChange = new EventEmitter();

  set selected(val) {
    this.selectedValue = val;
    this.selectedChange.emit(this.selectedValue);
    this.closeDropdown();
  }

  ngOnInit(): void {
    this.subscriptions.add(
      this.showDropdown.subscribe(showDropdown => {
        if (showDropdown) {
          this.filteredOptions = this.options;
          setTimeout(() => this.filterInput.nativeElement.focus(), 0);
        }
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

  toggleDropdown() {
    if (!this.disabled)
      this.showDropdown.next(!this.showDropdown.value)
  }

  closeDropdown() {
    if (this.showDropdown.value)
      this.showDropdown.next(false);
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe()
  }
}
