using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Reporte
{
    public class ReporteConflictoCampoSustentable : ReporteBase
    {
        public IList<ConflictoCampoSustentable> Campos { get; set; }

        public override string GetBody()
        {
            var bodyBuilder = new StringBuilder();

            foreach (var campo in Campos)
            {
                bodyBuilder.AppendLine($"<tr><td>{campo.IdTSA}</td><td>{campo.CUIT}</td><td>{campo.StockDisponible}</td><td>{campo.ToneladasActuales}</td><td>{campo.ToneladasInformadas}</td></tr>");
            }

            return bodyBuilder.ToString();
        }

        public override string GetFecha()
        {
            return DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
