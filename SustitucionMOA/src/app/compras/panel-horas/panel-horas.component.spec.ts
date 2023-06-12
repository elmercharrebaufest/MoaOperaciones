import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PanelHorasComponent } from './panel-horas.component';

describe('PanelHorasComponent', () => {
  let component: PanelHorasComponent;
  let fixture: ComponentFixture<PanelHorasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PanelHorasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PanelHorasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
