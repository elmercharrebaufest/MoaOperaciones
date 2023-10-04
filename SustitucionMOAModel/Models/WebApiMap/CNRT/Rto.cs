using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WebApiMap.CNRT
{
    public class Rto
    {
        public int Id { get; set; }

        public string NroPlanilla { get; set; }

        public int CodigoTaller { get; set; }

        public int CodigoAuditoria { get; set; }

        public string Dominio { get; set; }

        public string PaisRadicacion { get; set; }

        public DateTime FechaRevision { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime FechaFinVigencia { get; set; }

        public string CodigoTipoUso { get; set; }

        public string Resultado { get; set; }

        public string SerieCertificado { get; set; }

        public string NroCertificado { get; set; }

        public string CodigoTipoVehiculo { get; set; }

        public object TipoVehiculo { get; set; }

        public string CategoriaTecnica { get; set; }

        public string ConfiguracionEjes { get; set; }

        public int CantEjes { get; set; }

        public object MarcaTacografo { get; set; }

        public object NroTacografo { get; set; }

        public string CodigoTipoCarroceria { get; set; }

        public string TipoCarroceria { get; set; }

        public bool CargasPeligrosas { get; set; }

        public string MarcaChasis { get; set; }

        public string NroChasis { get; set; }

        public string NroMotor { get; set; }

        public List<string> ClasesCarga { get; set; }

        public List<object> ClasesServicio { get; set; }

        public string VersionRto { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }

        public DateTime FechaProceso { get; set; }
    }
}
