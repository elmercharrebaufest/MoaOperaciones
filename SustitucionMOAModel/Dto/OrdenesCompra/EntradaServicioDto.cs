using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    /// <summary>
    /// Este dto se utiliza en la vista de Ordenes de Compra
    /// </summary>
    public class EntradaServicioDto
    {
        public int Id { get; set; } //HEADER.SHEET_NO
        public string itemNumero { get; set; }
        //public string ItemId { get; set; }
        public string Fecha { get; set; } //HEADER.CREATED_ON
        public decimal? Cantidad { get; set; } //QUANTITY
        public DateTime FechaDocumento { get; set; } //HEADER.DOC_DATE
        //public List<string> Referencia { get; set; } 
        public string TextoBreve { get; set; } //HEADER.SHORT_TEXT
        public string ESS_PCKG_NO { get; set; }
        public string ESS_LINE_NO { get; set; }
        public string ESS_EXT_LINE { get; set; }
        
        //MMSN-460 - Añadir Ref_DOC_No
        public string Referencia { get; set; }//HEADER.REF_DOC_NO

        //MMSN-460 - Añadir Currency
        public string ImporteARPUSD { get; set; } //HEADER.CURRENCY

        //MMSN-460 - FechaDocumento p/ FE
        public string FechaDocumentoString { get; set; }

        //MMSN-460 - Fecha Contabilizacion
        public string FechaContabilizacion { get; set; }

        public bool SePuedeBorrar { get; set; }

        public string TemporalId { get; set; }

        public List<ItemEntradaServicioDto> Items { get; set; }
    }
}
