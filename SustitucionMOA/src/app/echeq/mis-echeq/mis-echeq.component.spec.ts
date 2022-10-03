import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MisEcheqComponent } from './mis-echeq.component';

describe('MisEcheqComponent', () => {
  let component: MisEcheqComponent;
  let fixture: ComponentFixture<MisEcheqComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MisEcheqComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MisEcheqComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
