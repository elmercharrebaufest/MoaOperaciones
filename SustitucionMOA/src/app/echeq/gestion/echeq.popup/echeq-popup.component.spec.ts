import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EcheqPopupComponent } from './echeq-popup.component';

describe('Echeq.PopupComponent', () => {
  let component: EcheqPopupComponent;
  let fixture: ComponentFixture<EcheqPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EcheqPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EcheqPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
