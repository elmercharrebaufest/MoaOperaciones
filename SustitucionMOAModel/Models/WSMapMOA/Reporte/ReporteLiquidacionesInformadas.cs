using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Reporte
{
    public class ReporteLiquidacionesInformadas: ReporteBase
    {
        public IList<LiquidacionInformada> Liquidaciones { get; set; }

        public override string GetBody()
        {
            var bodyBuilder = new StringBuilder();

            foreach (var liquidacion in Liquidaciones)
            {
                bodyBuilder.AppendLine($"<tr><td>{liquidacion.COE}</td><td>{(liquidacion.FechaComprobante.HasValue ? liquidacion.FechaComprobante.Value.ToString("dd-MM-yyyy") : "")}</td></tr>");
            }

            return bodyBuilder.ToString();
        }

        public override string GetFecha()
        {
            return Liquidaciones.First().FechaInformada.ToString("dd-MM-yyyy");
        }
    }
}
