import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuarioAuditoriaListComponent } from './usuario-auditoria-list.component';

describe('UsuarioAuditoriaListComponent', () => {
  let component: UsuarioAuditoriaListComponent;
  let fixture: ComponentFixture<UsuarioAuditoriaListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsuarioAuditoriaListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsuarioAuditoriaListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
