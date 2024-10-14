using System;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public abstract class OrdenDeCargaFasonRequest
    {
        public int Id { get; set; }

        public int Cantidad { get; set; }

        public int CantidadDeViajes { get; set; }

        public string ChasisAcoplado { get; set; }

        public int Cliente { get; set; }

        public string CUILChofer { get; set; }

        public long CUITCliente { get; set; }

        public string CUITTransporte { get; set; }

        public string NombreChofer { get; set; }

        public string Observacion { get; set; }

        public string PatenteAcoplado { get; set; }

        public string PatenteChasis { get; set; }

        public int Producto_Id { get; set; }
        public CrearOrdenDeCargaFasonRequestProducto ProductoSeleccionado { get; set; }

        public string RazonSocialTransporte { get; set; }

        public int? CorredorId { get; set; }

        public bool FleteMOA { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string RazonSocialIntermediarioFlete { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public bool Escalable { get; set; }

        public string CUITDestinatario { get; set; }

        public string RazonSocialDestinatario { get; set; }

        public string CUITDestino { get; set; }

        public string RazonSocialDestino { get; set; }

        public bool? DestinatarioExisteScato { get; set; }
        public bool? DestinoExisteScato { get; set; }
        public string DestinoMercaderia { get; set; }
        public string CodigoCorredor { get; set; }
    }

    public class CrearOrdenDeCargaFasonRequestProducto
    {
        public int MaterialId { get; set; }
        public string Descripcion { get; set; }
        public string CampaniaActual { get; set; }
        public int CampaniaIdActual { get; set; }
        public string CampaniaTablero { get; set; }
        public int CampaniaTableroId { get; set; }
        public string Codigo { get; set; }
        public int CodigoSap { get; set; }
        public bool ValidaSisaRuca { get; set; }
    }

}
