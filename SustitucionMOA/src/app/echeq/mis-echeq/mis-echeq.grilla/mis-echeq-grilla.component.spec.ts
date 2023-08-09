import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MisEcheqGrillaComponent } from './mis-echeq-grilla.component';

describe('MisEcheqGrillaComponent', () => {
  let component: MisEcheqGrillaComponent;
  let fixture: ComponentFixture<MisEcheqGrillaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MisEcheqGrillaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MisEcheqGrillaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
