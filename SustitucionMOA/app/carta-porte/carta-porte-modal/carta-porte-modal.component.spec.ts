import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CartaPorteModalComponent } from './carta-porte-modal.component';

describe('CartaPorteModalComponent', () => {
  let component: CartaPorteModalComponent;
  let fixture: ComponentFixture<CartaPorteModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CartaPorteModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CartaPorteModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
