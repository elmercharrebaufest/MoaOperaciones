import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrarCursosComponent } from './administrar-cursos.component';

describe('AdministrarCursosComponent', () => {
  let component: AdministrarCursosComponent;
  let fixture: ComponentFixture<AdministrarCursosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdministrarCursosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrarCursosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
