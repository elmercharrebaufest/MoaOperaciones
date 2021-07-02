using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoComandosWebService;
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
        private readonly IScatoComandosConsumer scatoComandosConsumer;

        public CpOtrosPuertosController(IScatoComandosConsumer scatoComandosConsumer)
        {
            this.scatoComandosConsumer = scatoComandosConsumer;
        }

        [Authorize(Roles = "APIKEY")]
        public IHttpActionResult Post([FromBody] IList<CpOtrosPuertosDto> cps)
        {
            if(cps == null || cps.Count == 0)
            {
                return BadRequest("No hay elementos a procesar");
            }

            try
            {
                var dtos = cps.Select(x => new CartaPorteOtrosPuertosDto()
                {
                    NumeroCartaPorte = x.NroCartaPorte,
                    CEE = x.CEE,
                    CTG = x.CTG.ToString(), //cambiar a int
                    ChoferCuit = x.Chofer?.CUIT,
                    ChoferRazonSocial = x.Chofer?.RazonSocial,
                    CorredorCuit = x.Corredor?.CUIT,
                    CorredorRazonSocial = x.Corredor?.RazonSocial,
                    DescuentosKgsCalidad = x.Descuentos?.KgsCalidad.ToString(), //cambiar a int
                    DescuentosKgsHumedad = x.Descuentos?.KgsHumedad.ToString(), //cambiar a int
                    DescuentosKgsSecada = x.Descuentos?.KgsSecada.ToString(), //cambiar a int
                    DestinatarioCuit = x.Destinatario?.CUIT,
                    DestinatarioRazonSocial = x.Destinatario?.RazonSocial,
                    DestinoCuit = x.Destino?.CUIT,
                    DestinoRazonSocial = x.Destino?.RazonSocial,
                    DestinoPB = x.Destino?.PesoBruto.ToString(), //cambiar a int
                    DestinoPN = x.Destino?.PesoNeto.ToString(), //cambiar a int
                    DestinoPT = x.Destino?.PesoTara.ToString(), //cambiar a int
                    EntregadorCuit = x.Entregador?.CUIT,
                    EntregadorRazonSocial = x.Entregador?.RazonSocial,
                    Establecimiento = x.Establecimiento,
                    FechaArribo = x.FechaArribo,
                    FechaCarga = x.FechaCarga,
                    FechaDescarga = x.FechaDescarga,
                    FechaVencimiento = x.FechaVencimiento,
                    IntermediarioCuit = x.Intermediario?.CUIT,
                    IntermediarioRazonSocial = x.Intermediario?.RazonSocial,
                    MermaTotal = x.MermaTotal.ToString(), //cambiar a int
                    NetoConvenido = x.NetoConvenido.ToString(), //cambiar a int
                    Planta = x.Planta,
                    ProcedenciaCodigo = x.Procedencia?.Codigo,
                    ProcedenciaLocalidad = x.Procedencia?.Localidad,
                    ProcedenciaProvincia = x.Procedencia?.Provincia,
                    ProcedenciaCP = x.Procedencia?.Cp,
                    ProcedenciaPB = x.Procedencia?.PesoBruto.ToString(), //cambiar a int
                    ProcedenciaPN = x.Procedencia?.PesoNeto.ToString(), //cambiar a int
                    ProcedenciaPT = x.Procedencia?.PesoTara.ToString(), //cambiar a int
                    ProductoCodigo = x.Producto?.Codigo,
                    ProductoCosecha = x.Producto?.Cosecha,
                    ProductoDescripcion = x.Producto?.Descripcion,
                    RemitenteCuit = x.RemitenteComercial?.CUIT,
                    RemitenteRazonSocial = x.RemitenteComercial.RazonSocial,
                    TitularCuit = x.Titular?.CUIT,
                    TitularRazonSocial = x.Titular?.RazonSocial,
                    TransporteCuit = x.Transporte?.CUIT,
                    TransporteRazonSocial = x.Transporte?.RazonSocial,
                    TrasportePatente = x.Transporte?.Patente,
                    FotoCpBase64 = x.FotoCp?.ImagenBase64,
                    FotoCpNombreArchivo = x.FotoCp?.NombreArchivo,
                    
                    Caracteristicas = x.Calidad.Select(y=> new CaracteristicasCartaPorteOtrosPuertosDto()
                    {
                        CodigoExterno = y.Codigo,
                        Valor = y.Resultado 
                    }).ToArray()
                }).ToList();

                var result = scatoComandosConsumer.CrearCpsOtrosPuertos(dtos, HttpContext.Current.User.Identity.Name);

                if(result.Errores.Count > 0)
                {
                    var ex = new Exception(string.Format("Se dieron los siguientes errores al procesar los elementos: {0}", string.Join(" | ", result.Errores.ToArray())));
                    return InternalServerError(ex);
                }
            }
            catch(Exception ex)
            {
                //loguear error
                return InternalServerError(new Exception("Hubo un error al procesar los elementos"));
            }

            return Ok();
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
        public decimal Resultado { get; set; }
    }

    public class CpOtrosPuertosDto
    {
        [Required]
        public string NroCartaPorte { get; set; }
        public DateTime FechaCarga { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaArribo { get; set; }
        public DateTime FechaDescarga { get; set; }
        public string CEE { get; set; }
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