import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabSubposicionComponent } from './tab-subposicion.component';

describe('TabSubposicionComponent', () => {
  let component: TabSubposicionComponent;
  let fixture: ComponentFixture<TabSubposicionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TabSubposicionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabSubposicionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
