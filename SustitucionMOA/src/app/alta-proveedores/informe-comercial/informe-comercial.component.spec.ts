import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InformeComercialComponent } from './informe-comercial.component';

describe('InformeComercialComponent', () => {
  let component: InformeComercialComponent;
  let fixture: ComponentFixture<InformeComercialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InformeComercialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformeComercialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
