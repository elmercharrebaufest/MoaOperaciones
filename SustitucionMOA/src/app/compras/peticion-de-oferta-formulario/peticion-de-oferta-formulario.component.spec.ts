import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { PeticionDeOfertaFormularioComponent } from './peticion-de-oferta-formulario.component';



describe('PeticionDeOfertaFormularioComponent', () => {
  let component: PeticionDeOfertaFormularioComponent;
  let fixture: ComponentFixture<PeticionDeOfertaFormularioComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PeticionDeOfertaFormularioComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PeticionDeOfertaFormularioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
