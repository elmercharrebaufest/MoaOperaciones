import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MisEcheqFiltrosComponent } from './mis-echeq-filtros.component';

describe('MisEcheqFiltrosComponent', () => {
  let component: MisEcheqFiltrosComponent;
  let fixture: ComponentFixture<MisEcheqFiltrosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MisEcheqFiltrosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MisEcheqFiltrosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
