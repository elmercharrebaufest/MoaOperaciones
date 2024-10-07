import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegajoComponent } from './legajo.component';

describe('LegajoComponent', () => {
  let component: LegajoComponent;
  let fixture: ComponentFixture<LegajoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegajoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegajoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
