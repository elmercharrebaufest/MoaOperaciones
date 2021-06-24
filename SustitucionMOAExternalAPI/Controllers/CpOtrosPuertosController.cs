using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class CpOtrosPuertosController : ApiController
    {
        [Authorize(Roles = "API")]
        public void Post([FromBody] IList<CpOtrosPuertosDto> cps)
        {
        }
    }

    public class ProductoDto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Cosecha { get; set; }
    }

    public class ProcedenciaDto
    {
        public string Codigo { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Cp { get; set; }
        public int PesoBruto { get; set; }
        public int PesoTara { get; set; }
        public int PesoNeto { get; set; }
    }

    public class DestinoDto : ParticipanteDto
    {
        public int PesoBruto { get; set; }
        public int PesoTara { get; set; }
        public int PesoNeto { get; set; }
    }

    public class TransporteDto : ParticipanteDto
    {
        public string Patente { get; set; }
    }

    public class ParticipanteDto
    {
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
    }

    public class DescuentosDto
    {
        public int KgsHumedad { get; set; }
        public int KgsCalidad { get; set; }
        public int KgsSecada { get; set; }
    }

    public class FotoCpDto
    {
        public string NombreArchivo { get; set; }
        public string ImagenBase64 { get; set; }
    }

    public class CaracteristicaDto
    {
        public string Codigo { get; set; }
        public double Resultado { get; set; }
    }

    public class CpOtrosPuertosDto
    {
        [Required]
        public string NroCartaPorte { get; set; }
        public DateTime FechaCarga { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaArribo { get; set; }
        public DateTime FechaDescarga { get; set; }
        public int CEE { get; set; }
        public int CTG { get; set; }
        public string Establecimiento { get; set; }
        public string Planta { get; set; }
        public ParticipanteDto Titular { get; set; }
        public ParticipanteDto Intermediario { get; set; }
        public ParticipanteDto RemitenteComercial { get; set; }
        public ProductoDto Producto { get; set; }
        public ProcedenciaDto Procedencia { get; set; }
        public ParticipanteDto Corredor { get; set; }
        public ParticipanteDto Entregador { get; set; }
        public ParticipanteDto Destinatario { get; set; }
        public DestinoDto Destino { get; set; }
        public TransporteDto Transporte { get; set; }
        public ParticipanteDto Chofer { get; set; }
        public int MermaTotal { get; set; }
        public int NetoConvenido { get; set; }
        public DescuentosDto Descuentos { get; set; }
        public FotoCpDto FotoCp { get; set; }
        public List<CaracteristicaDto> Calidad { get; set; }
    }


}