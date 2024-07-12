using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class ReporteDto
    {
        public string Id { get; set; }
        public string LINE_NO { get; set; }
        public int PosicionId { get; set; }
        public int NumeroLinea { get; set; }
        public double Cantidad { get; set; }
        public string Descripcion { get; set; }
        public int ServicioNumero { get; set; }
        public string UnidadMedida { get; set; }
        public decimal PrecioBruto { get; set; }
        public decimal Monto { get; set; }
        public string Toler { get; set; }
        public string ItemNumero { get; set; }
        public string SUBPCKG_NO { get; set; }
        public string UM { get; set; }
        public decimal Importe { get; set; }
        public string Moneda { get; set; }
        public decimal CantidadReal { get; set; }
        public string Porcentaje { get; set; }
        public string Solicitante { get; set; }
        public string ImporteString { get; set; }
        public string NroOrdenCompra { get; set; }
        public string NroPosicion { get; set; }
        public decimal CantidadACertificar { get; set; }
        public decimal PorcentajeACertificar { get; set; }
        public decimal MontoACertificar { get; set; }
        public bool isSelected { get; set; }
    }
}
