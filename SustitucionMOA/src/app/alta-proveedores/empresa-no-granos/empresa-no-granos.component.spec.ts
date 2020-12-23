import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpresaNoGranosComponent } from './empresa-no-granos.component';

describe('EmpresaNoGranosComponent', () => {
  let component: EmpresaNoGranosComponent;
  let fixture: ComponentFixture<EmpresaNoGranosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmpresaNoGranosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmpresaNoGranosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});