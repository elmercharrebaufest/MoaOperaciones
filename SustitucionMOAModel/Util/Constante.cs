using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Util
{
    public struct Constante
    {
        public const string FECHA_BASICA = "2015-01-01";
        public const int MESES_ATRAS_FAS = 12;
        public const TipoContratoFAS FAS_FILTRO_DEFAULT_TIPO_CONTRATO = TipoContratoFAS.Todos;
        public const bool FAS_FILTRO_DEFAULT_PENDIENTE = true;
        public const int FAS_KILOS_LIMITE_SUPERIOR = 15000;
        public const int FAS_KILOS_LIMITE_INFERIOR = 0;
    }
}
