using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Flete
{
    public class FletesWSMOAResponse
    {
        public List<Viaje> viajes { get; set; }
        public List<Proforma> proformas { get; set; }
        public ErrorWS error { get; set; }

        public FletesWSMOAResponse() {
            this.error = new ErrorWS();
            this.viajes = new List<Viaje>() { };
            this.proformas = new List<Proforma>() { };
        }
    }


    public class FletesAgrupadosWSMOAResponse
    {
        public List<ViajeAgrupado> viajes { get; set; }
        public List<Proforma> proformas { get; set; }
        public ErrorWS error { get; set; }

        public FletesAgrupadosWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.viajes = new List<ViajeAgrupado>() { };
            this.proformas = new List<Proforma>() { };
        }
    }

    public class FletesExcelWSMOAResponse
    {
        public List<ViajeExcel> viajes { get; set; }
        public List<Proforma> proformas { get; set; }
        public ErrorWS error { get; set; }

        public FletesExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.viajes = new List<ViajeExcel>() { };
            this.proformas = new List<Proforma>() { };
        }
    }


    public class FletesAgrupadosExcelWSMOAResponse
    {
        public List<ViajeAgrupadoExcel> viajes { get; set; }
        public List<Proforma> proformas { get; set; }
        public ErrorWS error { get; set; }

        public FletesAgrupadosExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.viajes = new List<ViajeAgrupadoExcel>() { };
            this.proformas = new List<Proforma>() { };
        }
    }
    

}
