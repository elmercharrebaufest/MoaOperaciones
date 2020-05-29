using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario
{
    public class CCPPFormulario
    {
        public string nroCCPP { get; set; }
        public string nroCEE { get; set; }
        public string nroCTG { get; set; }
        public string nroRenspa { get; set; }
        public string fechaCarga { get; set; }
        public string fechaVencimiento { get; set; }
        //public string titular { get; set; }
        //public string cuitTitular { get; set; }
        public string intermediario { get; set; }
        public string cuitIntermediario { get; set; }
        public string remitenteComercial { get; set; }
        public string cuitRemitenteComercial { get; set; }
        public string corredorComprador { get; set; }
        public string cuitCorredorComprador { get; set; }
        public string mercadoATermino { get; set; }
        public string cuitMercadoATermino { get; set; }
        public string corredorVendedor { get; set; }
        public string cuitCorredorVendedor { get; set; }
        public string representanteEntregador { get; set; }
        public string cuitRepresentanteEntregador { get; set; }
        public string destinatario { get; set; }
        public string cuitDestinatario { get; set; }
        public string destino { get; set; }
        public string cuitDestino { get; set; }
        public string intermediarioFlete { get; set; }
        public string cuitIntermediarioFlete { get; set; }
        public string transportista { get; set; }
        public string cuitTransportista { get; set; }
        public string chofer { get; set; }
        public string cuitCuilChofer { get; set; }
        public string granoEspecie { get; set; }
        public string tipo { get; set; }
        public string cosecha { get; set; }
        public string nroContrato { get; set; }
        public string cargaPesadaDestino { get; set; }
        public string kilosEstimados { get; set; }
        public string declaracionCalidad { get; set; }
        public string conforme { get; set; }
        public string condicional { get; set; }
        public string pesoBruto { get; set; }
        public string pesoTara { get; set; }
        public string pesoNeto { get; set; }
        public string observaciones { get; set; }
        public string procedenciaMercaderia { get; set; }
        public string establecimiento { get; set; }
        public string direccion { get; set; }
        public string direccionDestino { get; set; }
        public string pagadorFlete { get; set; }
        public string camion1 { get; set; }
        public string camion2 { get; set; }
        public string camion3 { get; set; }
        public string fletePagado { get; set; }
        public string fletePagar { get; set; }
        public string acoplado { get; set; }
        public string tarifaReferencia { get; set; }
        public string tarifa { get; set; }
        public string kmRecorrer { get; set; }
        public string localidad { get; set; }
        public string localidadDestino { get; set; }
        public string provincia { get; set; }
        public string provinciaDestino { get; set; }
    }
}
