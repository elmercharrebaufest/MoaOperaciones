import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AprobacionExternaComponent } from './aprobacion-externa.component';

describe('AprobacionExternaComponent', () => {
  let component: AprobacionExternaComponent;
  let fixture: ComponentFixture<AprobacionExternaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AprobacionExternaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AprobacionExternaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
