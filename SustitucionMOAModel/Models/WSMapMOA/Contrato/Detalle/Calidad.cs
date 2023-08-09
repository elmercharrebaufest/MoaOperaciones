using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Calidad
    {
        public string ccpp { get; set; }
        public string kgNetosTotal { get; set; }
        public string kgAplicadosTotal { get; set; }
        public string kgDtoTotal { get; set; }
        public string dtoPorcTotal { get; set; }
        public string certificado { get {
                if(!tieneCertificado)
                    return"";
                return registros.Find(cal => !string.IsNullOrEmpty(cal.certificado)).certificado;
        }}
        public string camaraAPresent { get; set; }
        public List<CalidadElement> registros { get; set; }
        private bool _camaraPendiente { get; set; }
        public bool tieneCertificado { get { return registros.Any(cal => !string.IsNullOrEmpty(cal.certificado)); } }
        public bool camaraPendiente { get { return _camaraPendiente; } }
        public void SetearEstadoCamara()
        {
            _camaraPendiente = !string.IsNullOrEmpty(camaraAPresent) && string.IsNullOrEmpty(certificado);
        }
    }
}
