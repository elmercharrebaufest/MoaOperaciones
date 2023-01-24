using System.Collections.Generic;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Calidad
    {
        public string ccpp { get; set; }
        public string kgNetosTotal { get; set; }
        public string kgAplicadosTotal { get; set; }
        public string kgDtoTotal { get; set; }
        public string dtoPorcTotal { get; set; }
        public string certificado { get; set; }
        public string camaraAPresent { get; set; }
        public List<CalidadElement> registros { get; set; }
        private bool _camaraPendiente { get; set; }
        public bool camaraPendiente { get { return _camaraPendiente; } }
        public void SetearEstadoCamara()
        {
            _camaraPendiente = !string.IsNullOrEmpty(camaraAPresent) && string.IsNullOrEmpty(certificado);
        }
    }
}
