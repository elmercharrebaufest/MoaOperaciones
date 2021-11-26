import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabFechasComponent } from './tab-fechas.component';

describe('TabFechasComponent', () => {
  let component: TabFechasComponent;
  let fixture: ComponentFixture<TabFechasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TabFechasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabFechasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
