import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GestionEcheqComponent } from './gestion.component';

describe('GestionEcheqComponent', () => {
  let component: GestionEcheqComponent;
  let fixture: ComponentFixture<GestionEcheqComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GestionEcheqComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GestionEcheqComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
