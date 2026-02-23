import { ConsultaTicketPesadaModule } from './consulta-ticket-pesada.module';

describe('ConsultaTicketPesadaModule', () => {
  let consultaTicketPesadaModule: ConsultaTicketPesadaModule;

  beforeEach(() => {
    consultaTicketPesadaModule = new ConsultaTicketPesadaModule();
  });

  it('should create an instance', () => {
    expect(consultaTicketPesadaModule).toBeTruthy();
  });
});
