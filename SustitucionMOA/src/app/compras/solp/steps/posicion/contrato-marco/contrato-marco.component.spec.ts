import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContratoMarcoComponent } from './contrato-marco.component';

describe('ContratoMarcoComponent', () => {
  let component: ContratoMarcoComponent;
  let fixture: ComponentFixture<ContratoMarcoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContratoMarcoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContratoMarcoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
