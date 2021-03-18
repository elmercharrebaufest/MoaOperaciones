import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImpresionDeclaracionComponent } from './impresion-declaracion.component';

describe('ImpresionDeclaracionComponent', () => {
  let component: ImpresionDeclaracionComponent;
  let fixture: ComponentFixture<ImpresionDeclaracionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImpresionDeclaracionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImpresionDeclaracionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
