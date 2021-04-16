import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListadoCamposComponent } from './listado-campos.component';

describe('ListadoCamposComponent', () => {
  let component: ListadoCamposComponent;
  let fixture: ComponentFixture<ListadoCamposComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListadoCamposComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListadoCamposComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
