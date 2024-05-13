using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class OrdenCompraEntradaServicioDto
    {
        public int EntradaServicioId { get; set; } //HEADER.SHEET_NO
        public int ItemId { get; set; }
        public DateTime Fecha { get; set; } //HEADER.CREATED_ON
        public int Cantidad { get; set; }
        public DateTime FechaDocumento { get; set; } //HEADER.DOC_DATE
        public int Referencia { get; set; }
        public string TextoBreve { get; set; } //HEADER.SHORT_TEXT
    }
}
