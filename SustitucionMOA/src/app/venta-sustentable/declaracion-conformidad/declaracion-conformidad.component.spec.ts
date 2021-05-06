import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeclaracionConformidadComponent } from './declaracion-conformidad.component';

describe('DeclaracionConformidadComponent', () => {
  let component: DeclaracionConformidadComponent;
  let fixture: ComponentFixture<DeclaracionConformidadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeclaracionConformidadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeclaracionConformidadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
