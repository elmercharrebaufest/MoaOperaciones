import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgruparPoThComponent } from './agrupar-po-th.component';

describe('AgruparPoThComponent', () => {
  let component: AgruparPoThComponent;
  let fixture: ComponentFixture<AgruparPoThComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgruparPoThComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgruparPoThComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
