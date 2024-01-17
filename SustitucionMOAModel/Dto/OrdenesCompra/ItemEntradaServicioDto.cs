using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class ItemEntradaServicioDto
    {
        public string Id { get; set; }
        public decimal? Cantidad { get; set; }
        public string Descripcion { get; set; }


        public int? PosicionId { get; set; }
        public string Campo_I { get; set; }
        public int? ServicioNumero { get; set; }
        public string UnidadMedida { get; set; }
        public decimal? PrecioBruto { get; set; }
        public decimal Monto { get; set; }
        public string Toler { get; set; }
        public string ItemNumero { get; set; }
        public string SUBPCKG_NO { get; set; }
        public int NumeroLinea { get; set; }
        public string PCKG_NO { get; set; }
        public string LINE_NO { get; set; }
        public string PLN_PCKG { get; set; }
        public string PLN_LINE { get; set; }
        public string EXT_LINE { get; set; }
        //blic List<EntradaServicioDto> EntradasServicio { get; set; }

        //Elementos de Cabecera para Front End
        public string Fecha { get; set; } //HEADER.CREATED_ON

        public string FechaDocumento { get; set; } //HEADER.DOC_DATE

        public string Referencia { get; set; }//HEADER.REF_DOC_NO

        public string ImporteARPUSD { get; set; } //HEADER.CURRENCY

        public string FechaContabilizacion { get; set; } //HEADER.POST_DATE
    }
}
