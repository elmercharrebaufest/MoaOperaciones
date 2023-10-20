using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WebApiMap.CNRT
{
    public class Ruta
    {
        public int Id { get; set; }

        public string CuitEmpresa { get; set; }

        public string Dominio { get; set; }

        public DateTime FechaValidacion { get; set; }

        public DateTime FechaAlta { get; set; }

        public string NroCertificado { get; set; }

        public string NroConstancia { get; set; }

        public DateTime FechaVencimientoConstancia { get; set; }

        public DateTime FechaUltimaActualizacion { get; set; }

        public DateTime? FechaBaja { get; set; }

        public string CodigoTipoVehiculo { get; set; }

        public string CategoriaTecnica { get; set; }

        public string CodigoTipoCarga { get; set; }

        public int CantEjes { get; set; }

        public string TipoCaja { get; set; }

        public int AnioModelo { get; set; }

        public object MarcaCarroceria { get; set; }

        public string NroChasis { get; set; }

        public string MarcaChasis { get; set; }

        public string ModeloChasis { get; set; }

        public string MarcaMotor { get; set; }

        public bool Historico { get; set; }

        public DateTime FechaProceso { get; set; }
    }
}
