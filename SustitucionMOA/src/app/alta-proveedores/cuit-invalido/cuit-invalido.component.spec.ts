import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CuitInvalidoComponent } from './cuit-invalido.component';

describe('CuitInvalidoComponent', () => {
  let component: CuitInvalidoComponent;
  let fixture: ComponentFixture<CuitInvalidoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CuitInvalidoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CuitInvalidoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
