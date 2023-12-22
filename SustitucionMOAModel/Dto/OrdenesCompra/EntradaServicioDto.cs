using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioDto
    {
        public int Id { get; set; } //HEADER.SHEET_NO
        public string itemNumero { get; set; }
        //public string ItemId { get; set; }
        public string Fecha { get; set; } //HEADER.CREATED_ON
        public decimal? Cantidad { get; set; } //QUANTITY
        //public DateTime FechaDocumento { get; set; } //HEADER.DOC_DATE
        //public List<string> Referencia { get; set; } 
        public string TextoBreve { get; set; } //HEADER.SHORT_TEXT
        public string PCKG { get; set; }

        public List<ItemEntradaServicioDto> Items { get; set; }
    }
}
