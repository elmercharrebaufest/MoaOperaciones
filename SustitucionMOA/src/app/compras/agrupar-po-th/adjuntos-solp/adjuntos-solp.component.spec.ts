import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdjuntosSolpComponent } from './adjuntos-solp.component';

describe('AdjuntosSolpComponent', () => {
  let component: AdjuntosSolpComponent;
  let fixture: ComponentFixture<AdjuntosSolpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdjuntosSolpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdjuntosSolpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
