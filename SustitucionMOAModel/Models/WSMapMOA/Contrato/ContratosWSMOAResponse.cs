using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato
{
    public class ContratosWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<string> contratos { get; set; }
        public List<string> vendedores { get; set; }
        public List<string> cosechas { get; set; }
        public List<string> materiales { get; set; }
        public List<FechaStringWS> fechas { get; set; }
        public List<ContratoCumplidoView> contratosInfo { get; set; }

        public ContratosWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.contratos = new List<string>() { };
            this.vendedores = new List<string>() { };
            this.cosechas = new List<string>() { };
            this.materiales = new List<string>() { };
            this.fechas = new List<FechaStringWS>() { };
            this.contratosInfo = new List<ContratoCumplidoView>() { };
        }
    }

    public class ContratosExcelWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<string> contratos { get; set; }
        public List<string> vendedores { get; set; }
        public List<string> cosechas { get; set; }
        public List<string> materiales { get; set; }
        public List<FechaStringWS> fechas { get; set; }
        public List<ContratoCumplido> contratosInfo { get; set; }

        public ContratosExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.contratos = new List<string>() { };
            this.vendedores = new List<string>() { };
            this.cosechas = new List<string>() { };
            this.materiales = new List<string>() { };
            this.fechas = new List<FechaStringWS>() { };
            this.contratosInfo = new List<ContratoCumplido>() { };
        }
    }
}
