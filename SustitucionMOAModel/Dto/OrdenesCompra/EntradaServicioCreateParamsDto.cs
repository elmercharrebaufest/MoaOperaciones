using System;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{

    public class CreateEntradaServicioDto
    {
        public List<EntradaServicioCreateParamsDto> Posiciones { get; set; }
        public List<ReporteDto> report { get; set; }
        public List<string> IdAdjuntos {  get; set; }
    }

    public class EntradaServicioCreateParamsDto
    {
        public EntrySheetHeaderSection EntrySheetHeader { get; set; }
        public EntrySheetServiceSection EntrySheetServices { get; set; }
    }


    public class EntrySheetHeaderSection
    {
        public List<string> SolPedNumber { get; set; } //MMSN-601
        //MMSN-602
        public string MontoTotalACertificar { get; set; }
        public string PaqueteNumero { get; set; } // se puede omitir del front
        public string Descripcion { get; set; }
        public string OrdenCompraNumero { get; set; }
        public string OrdenCompraPosicionNumero { get; set; }
        public string DocumentoReferenciaNumero { get; set; }
        public string FechaDocumento { get; set; }
        public string FechaContabilizacion { get; set; }
        public string GrabarAceptada { get; set; } // se puede omitir del front
        public string Proveedor { get; set; }
    }


    public class EntrySheetServiceSection
    {
        public List<EntrySheetServiceItemSection> Items { get; set; }
    }


    public class EntrySheetServiceItemSection
    {
        public string PackageNumber { get; set; } // se puede omitir del front
        public string LineNumber { get; set; } // se puede omitir del front
        public string OutlineIndicator { get; set; } // se puede omitir del front
        public string SubPackageNumber { get; set; } // se puede omitir del front
        public string ExternalLineNumber { get; set; }
        public string Service { get; set; }
        public string Quantity { get; set; }
        public string ItemQuantity { get; set; }
        public string UM { get; set; }
        public string ItemGrossPrice { get; set; }
        public decimal GrossPrice { get; set; }
        public string Percentage { get; set; }
        public string CertificationAmount { get; set; }
        public string ShortText { get; set; }
        public string PlannedPackage { get; set; }
        public string PlannedLine { get; set; }
        public string Descripcion { get; set; }
    }
}
