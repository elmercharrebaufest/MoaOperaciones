namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ValidaSisaCuit
    {
        private readonly string _CuitDestino = "CUITDestino";
        private readonly string _CuitDestinatario = "CUITDestinatario";
        private readonly string _campo;

        public ValidaSisaCuit(string campo)
        {
            _campo = campo;
        }

        public bool Destinatario
        {
            get
            {
                return _campo == _CuitDestinatario;
            }
        }

        public bool Destino
        {
            get
            {
                return _campo == _CuitDestino;

            }
        }
    }
}
