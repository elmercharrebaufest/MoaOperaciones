import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpresaGranosComponent } from './empresa-granos.component';

describe('EmpresaGranosComponent', () => {
  let component: EmpresaGranosComponent;
  let fixture: ComponentFixture<EmpresaGranosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmpresaGranosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmpresaGranosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
