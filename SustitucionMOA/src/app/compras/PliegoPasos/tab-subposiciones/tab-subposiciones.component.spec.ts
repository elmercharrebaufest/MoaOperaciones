import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabSubposicionesComponent } from './tab-subposiciones.component';

describe('TabSubposicionesComponent', () => {
  let component: TabSubposicionesComponent;
  let fixture: ComponentFixture<TabSubposicionesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TabSubposicionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabSubposicionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
