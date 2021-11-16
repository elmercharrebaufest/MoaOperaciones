import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabImputacionesComponent } from './tab-imputaciones.component';

describe('TabImputacionesComponent', () => {
  let component: TabImputacionesComponent;
  let fixture: ComponentFixture<TabImputacionesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TabImputacionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabImputacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
