import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReporteOcComponent } from './reporte-oc.component';

describe('ReporteOcComponent', () => {
  let component: ReporteOcComponent;
  let fixture: ComponentFixture<ReporteOcComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReporteOcComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReporteOcComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
