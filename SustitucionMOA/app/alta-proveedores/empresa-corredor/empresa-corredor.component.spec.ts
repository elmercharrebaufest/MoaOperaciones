import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpresaCorredorComponent } from './empresa-corredor.component';

describe('EmpresaCorredorComponent', () => {
  let component: EmpresaCorredorComponent;
  let fixture: ComponentFixture<EmpresaCorredorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmpresaCorredorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmpresaCorredorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
