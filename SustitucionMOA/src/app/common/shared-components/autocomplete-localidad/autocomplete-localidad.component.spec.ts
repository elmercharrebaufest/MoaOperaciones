import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AutocompleteLocalidadComponent } from './autocomplete-localidad.component';

describe('AutocompleteLocalidadComponent', () => {
  let component: AutocompleteLocalidadComponent;
  let fixture: ComponentFixture<AutocompleteLocalidadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AutocompleteLocalidadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutocompleteLocalidadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
