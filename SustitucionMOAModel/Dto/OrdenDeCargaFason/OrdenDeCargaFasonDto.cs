using Newtonsoft.Json;
using Ent = SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class OrdenDeCargaFasonDto
    {
        //[JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("cliente")]

        public string CUITCliente { get; set; }

        [JsonProperty("estado")]
        public EstadoOrdenDeCargaFason Estado { get; set; }
        public string DescripcionEstado { get; set; }
        public string DescripcionEstadoListado { get; set; }
        public string ColorSemaforo { get; set; }
        public string Material { get; set; }
        public string Cliente { get; set; }
        public string RazonSocialCliente { get; set; }
        public string Corredor { get; set; }
        public string RazonSocialCorredor { get; set; }

        [JsonProperty("fechaCreacion")]
        public string FechaCreacion { get; set; }

        [JsonProperty("fechaRetiro")]
        public string FechaRetiro { get; set; }
        public int Cantidad { get; set; }
        public string PatenteAcoplado { get; set; }
        public string NombreChofer { get; set; }
        public string CUILChofer { get; set; }
        public string RazonSocialTransporte { get; set; }
        public string CUITTransporte { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string RazonSocialIntermediarioFlete { get; set; }

        [JsonProperty("patenteChasis")]
        public string PatenteChasis { get; set; }

        public bool TransporteExiste { get; set; }
        public string Observacion { get; set; }
        public bool FleteMOA { get; set; }
        public int LocalidadId { get; set; }
        public string LocalidadDescripcion { get; set; }
        public Models.DataAgro.MaterialDto ProductoSeleccionado { get; set; }
        public int Producto_Id { get; set; }
        public bool Reventa { get; set; }
        public bool Escalable { get; set; }

        public bool ValidaSisaRuca { get; set; }
        public string CUITDestino { get; set; }
        public string CUITDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioDescr { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public bool NecesitaVerificarCuitsTerceros { get; set; }

        public OrdenDeCargaFasonDto(Ent.OrdenDeCargaFason orden, bool esInterno)
        {
            Cantidad = orden.Cantidad;
            Cliente = orden.Cliente.CodigoProveedor;
            ColorSemaforo = orden.Estado.ObtenerSemaforo();
            Corredor = orden.Corredor?.CodigoProveedor;
            CUILChofer = orden.CUILChofer;
            CUITCliente = "";
            CUITTransporte = orden.CUITTransporte;
            DescripcionEstado = esInterno ? orden.Estado.ToFriendlyStringInterno() : orden.Estado.ToFriendlyStringExterno();
            DescripcionEstadoListado = "";
            Estado = orden.Estado;
            FechaCreacion = orden.FechaCreacion.ToString("dd/MM/yyyy HH:mm");
            FechaRetiro = orden.FechaRetiro.ToString("dd/MM/yyyy");
            Id = orden.Id;
            LocalidadDescripcion = orden.LocalidadDescripcion;
            LocalidadId = orden.LocalidadId;
            Material = orden.Producto.Nombre;
            NombreChofer = orden.NombreChofer;
            Producto_Id = orden.Producto.Id;
            ProductoSeleccionado = new SustitucionMOAModel.Models.DataAgro.MaterialDto
            {
                MaterialId = orden.Producto.Id,
                Descripcion = orden.Producto.Nombre,
                CodigoSap = orden.Producto.CodigoSap,
            };
            PatenteAcoplado = orden.PatenteAcoplado;
            PatenteChasis = orden.PatenteChasis;
            RazonSocialCliente = orden.Cliente.RazonSocial;
            RazonSocialCorredor = orden.Corredor?.RazonSocial;
            RazonSocialTransporte = orden.RazonSocialTransporte;
            TransporteExiste = orden.TransporteExiste;
            Observacion = orden.Observacion;
            CUITIntermediarioFlete = orden.CUITIntermediarioFlete;
            RazonSocialIntermediarioFlete = orden.RazonSocialIntermediarioFlete;
            FleteMOA = orden.FleteMOA;
            Reventa = orden.Reventa;
            Escalable = orden.Escalable;
            ValidaSisaRuca = orden.Producto.ValidaSisaRuca;
            CUITDestino = orden.CUITDestino;
            CUITDestinatario = orden.CUITDestinatario;
            RazonSocialDestino = orden.RazonSocialDestino;
            RazonSocialDestinatario = orden.RazonSocialDestinatario;
            PlantaCodigo = orden.PlantaCodigo;
            DomicilioDescr = orden.DomicilioDescr;
            DomicilioTipo = orden.DomicilioTipo;
            DomicilioOrden = orden.DomicilioOrden;
            NecesitaVerificarCuitsTerceros = !orden.CuitsTerceroExisten;
        }
    }
}
