using Kendo.DynamicLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SustitucionMOAUtils.Services
{
    public class CrearContratoService : ICrearContratoService
    {

        protected readonly IRepositorio repositorio;
        private readonly string DataAgroURL;
        public CrearContratoService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
        }

        public string CrearContratoAPrecio(ContratoAPrecio contratoAPrecio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarContratoAPrecio");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/GrabarContratoAPrecio";
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(contratoAPrecio);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string ObteneDatosContrato()
        {
            var pruebalocal = true;
            if (pruebalocal)
            {
                var item = "{\"Datos\":{\"moneda\":[{\"MonedaId\":\"ARP  \",\"Descripcion\":\"ARP\"},{\"MonedaId\":\"USDM \",\"Descripcion\":\"USD\"}],\"tiponegocio\":[{\"TipoNegocioId\":1,\"Descripcion\":\"A FIJAR\"},{\"TipoNegocioId\":2,\"Descripcion\":\"A PRECIO\"},{\"TipoNegocioId\":3,\"Descripcion\":\"FIJACION\"}],\"material\":[{\"MaterialId\":1,\"Codigo\":null,\"Descripcion\":\"Maiz\",\"CampañaIdActual\":0,\"CampaniaTableroId\":0},{\"MaterialId\":2,\"Codigo\":null,\"Descripcion\":\"Trigo\",\"CampañaIdActual\":0,\"CampaniaTableroId\":0},{\"MaterialId\":3,\"Codigo\":null,\"Descripcion\":\"Soja\",\"CampañaIdActual\":0,\"CampaniaTableroId\":0},{\"MaterialId\":4,\"Codigo\":null,\"Descripcion\":\"Girasol\",\"CampañaIdActual\":0,\"CampaniaTableroId\":0},{\"MaterialId\":5,\"Codigo\":null,\"Descripcion\":\"Girsol AO\",\"CampañaIdActual\":0,\"CampaniaTableroId\":0}],\"prov\":[{\"Provinciaid\":1,\"Nombre\":\"BUENOS AIRES\",\"Orden\":1},{\"Provinciaid\":3,\"Nombre\":\"CORDOBA\",\"Orden\":2},{\"Provinciaid\":12,\"Nombre\":\"SANTA FE\",\"Orden\":3},{\"Provinciaid\":5,\"Nombre\":\"ENTRE RIOS\",\"Orden\":4},{\"Provinciaid\":21,\"Nombre\":\"LA PAMPA\",\"Orden\":5},{\"Provinciaid\":13,\"Nombre\":\"SANTIAGO DEL ESTERO\",\"Orden\":6},{\"Provinciaid\":16,\"Nombre\":\"CHACO\",\"Orden\":7},{\"Provinciaid\":9,\"Nombre\":\"SALTA\",\"Orden\":8},{\"Provinciaid\":14,\"Nombre\":\"TUCUMAN\",\"Orden\":9},{\"Provinciaid\":0,\"Nombre\":\"CAPITAL FEDERAL\",\"Orden\":10},{\"Provinciaid\":2,\"Nombre\":\"CATAMARCA\",\"Orden\":11},{\"Provinciaid\":4,\"Nombre\":\"CORRIENTES\",\"Orden\":12},{\"Provinciaid\":6,\"Nombre\":\"JUJUY\",\"Orden\":13},{\"Provinciaid\":7,\"Nombre\":\"MENDOZA\",\"Orden\":14},{\"Provinciaid\":8,\"Nombre\":\"LA RIOJA\",\"Orden\":15},{\"Provinciaid\":10,\"Nombre\":\"SAN JUAN\",\"Orden\":16},{\"Provinciaid\":11,\"Nombre\":\"SAN LUIS\",\"Orden\":17},{\"Provinciaid\":17,\"Nombre\":\"CHUBUT\",\"Orden\":18},{\"Provinciaid\":18,\"Nombre\":\"FORMOSA\",\"Orden\":19},{\"Provinciaid\":19,\"Nombre\":\"MISIONES\",\"Orden\":20},{\"Provinciaid\":20,\"Nombre\":\"NEUQUEN\",\"Orden\":21},{\"Provinciaid\":22,\"Nombre\":\"RIO NEGRO\",\"Orden\":22},{\"Provinciaid\":23,\"Nombre\":\"SANTA CRUZ\",\"Orden\":23},{\"Provinciaid\":24,\"Nombre\":\"TIERRA DEL FUEGO\",\"Orden\":24}],\"loc\":[],\"comercial\":[{\"ComercialId\":3,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\" Bilbao Lema Gustavo\",\"EmpleadorACargo\":null},{\"ComercialId\":5,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\" Canteli Manuel\",\"EmpleadorACargo\":null},{\"ComercialId\":31,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Adalberto Fratini Tomassoni\",\"EmpleadorACargo\":null},{\"ComercialId\":63,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Alan Modena \",\"EmpleadorACargo\":null},{\"ComercialId\":39,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Alberto Ricardo Belladelli\",\"EmpleadorACargo\":null},{\"ComercialId\":47,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Alejandra Florencia Simondi\",\"EmpleadorACargo\":null},{\"ComercialId\":97,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Alejandra Rodriguez\",\"EmpleadorACargo\":null},{\"ComercialId\":78,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Alejandra Sarquis\",\"EmpleadorACargo\":null},{\"ComercialId\":82,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Alejandro Baeza\",\"EmpleadorACargo\":null},{\"ComercialId\":57,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Alejandro Olivera\",\"EmpleadorACargo\":null},{\"ComercialId\":21,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Alvaro Gonzalez Del Cerro\",\"EmpleadorACargo\":null},{\"ComercialId\":11,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Andrea Lopez \",\"EmpleadorACargo\":null},{\"ComercialId\":112,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Bonnie Aldana\",\"EmpleadorACargo\":null},{\"ComercialId\":116,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Brisa Melgarejo\",\"EmpleadorACargo\":null},{\"ComercialId\":15,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Carina Scasso \",\"EmpleadorACargo\":null},{\"ComercialId\":109,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Carlos Casas\",\"EmpleadorACargo\":null},{\"ComercialId\":96,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Carolina Cascio\",\"EmpleadorACargo\":null},{\"ComercialId\":95,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Cesar Panisa\",\"EmpleadorACargo\":null},{\"ComercialId\":22,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Claudio Ladogana\",\"EmpleadorACargo\":null},{\"ComercialId\":40,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Claudio Oscar Leonori\",\"EmpleadorACargo\":null},{\"ComercialId\":104,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Claudio Paz\",\"EmpleadorACargo\":null},{\"ComercialId\":87,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Cristian Cabral\",\"EmpleadorACargo\":null},{\"ComercialId\":100,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Daniela Osorio\",\"EmpleadorACargo\":null},{\"ComercialId\":44,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Data Agro\",\"EmpleadorACargo\":null},{\"ComercialId\":27,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"David Ivan Erbojo\",\"EmpleadorACargo\":null},{\"ComercialId\":119,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Diego Garcia\",\"EmpleadorACargo\":null},{\"ComercialId\":75,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Diego Margutti\",\"EmpleadorACargo\":null},{\"ComercialId\":41,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Diego Martin Lerini\",\"EmpleadorACargo\":null},{\"ComercialId\":13,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Emilio Agustin Melo \",\"EmpleadorACargo\":null},{\"ComercialId\":115,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Eugenio Martin\",\"EmpleadorACargo\":null},{\"ComercialId\":18,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Federico Vargas \",\"EmpleadorACargo\":null},{\"ComercialId\":36,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Fernando Dianda\",\"EmpleadorACargo\":null},{\"ComercialId\":50,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Fernando Ponce de Leon\",\"EmpleadorACargo\":null},{\"ComercialId\":102,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Florencia Belen Martinez\",\"EmpleadorACargo\":null},{\"ComercialId\":114,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Florencia Gomez\",\"EmpleadorACargo\":null},{\"ComercialId\":16,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Gaston Alejandro Talano \",\"EmpleadorACargo\":null},{\"ComercialId\":92,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Gisela Toso\",\"EmpleadorACargo\":null},{\"ComercialId\":30,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Glenda Lien\",\"EmpleadorACargo\":null},{\"ComercialId\":2,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Gustavo Batchilleria\",\"EmpleadorACargo\":null},{\"ComercialId\":38,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Haydee Elida Bentivoglio\",\"EmpleadorACargo\":null},{\"ComercialId\":10,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Hernan Laso \",\"EmpleadorACargo\":null},{\"ComercialId\":28,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Ignacio De Anchorena\",\"EmpleadorACargo\":null},{\"ComercialId\":68,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Isabel Sienra\",\"EmpleadorACargo\":null},{\"ComercialId\":111,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Ivan Curtis\",\"EmpleadorACargo\":null},{\"ComercialId\":33,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Ivan Pasman\",\"EmpleadorACargo\":null},{\"ComercialId\":24,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Jaime Ferguson\",\"EmpleadorACargo\":null},{\"ComercialId\":88,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Joaquín Del Federico\",\"EmpleadorACargo\":null},{\"ComercialId\":7,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Juan Carlos Cappello\",\"EmpleadorACargo\":null},{\"ComercialId\":73,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Juan D Antonio\",\"EmpleadorACargo\":null},{\"ComercialId\":103,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Juan Fernandez\",\"EmpleadorACargo\":null},{\"ComercialId\":74,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Juan Funes\",\"EmpleadorACargo\":null},{\"ComercialId\":45,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"LAURA NUÑEZ\",\"EmpleadorACargo\":null},{\"ComercialId\":8,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Leandro Martin Fiore \",\"EmpleadorACargo\":null},{\"ComercialId\":56,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Leonel Magallanes\",\"EmpleadorACargo\":null},{\"ComercialId\":4,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Lucas Hernan Canosa\",\"EmpleadorACargo\":null},{\"ComercialId\":17,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Marcelo Gerardo Valoni \",\"EmpleadorACargo\":null},{\"ComercialId\":120,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Marcelo Spahm\",\"EmpleadorACargo\":null},{\"ComercialId\":48,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Marcos Venturini\",\"EmpleadorACargo\":null},{\"ComercialId\":117,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Maria Gobelli\",\"EmpleadorACargo\":null},{\"ComercialId\":84,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Marian Rabellato\",\"EmpleadorACargo\":null},{\"ComercialId\":34,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Matias Robiolo\",\"EmpleadorACargo\":null},{\"ComercialId\":101,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Mauricio Campero\",\"EmpleadorACargo\":null},{\"ComercialId\":105,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Miguel Castillo\",\"EmpleadorACargo\":null},{\"ComercialId\":72,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Milagros Labandibar\",\"EmpleadorACargo\":null},{\"ComercialId\":91,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Myrian Vidal\",\"EmpleadorACargo\":null},{\"ComercialId\":89,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Natalia Glaria\",\"EmpleadorACargo\":null},{\"ComercialId\":76,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Nicolas Descole\",\"EmpleadorACargo\":null},{\"ComercialId\":35,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Nicolas Gullini\",\"EmpleadorACargo\":null},{\"ComercialId\":93,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Omar Olmos\",\"EmpleadorACargo\":null},{\"ComercialId\":19,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Pablo Jesus Vicente \",\"EmpleadorACargo\":null},{\"ComercialId\":110,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Pablo Poggi\",\"EmpleadorACargo\":null},{\"ComercialId\":83,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Paolo Magrini\",\"EmpleadorACargo\":null},{\"ComercialId\":37,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Rafael Alfredo Simal\",\"EmpleadorACargo\":null},{\"ComercialId\":14,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Raul Montorfano \",\"EmpleadorACargo\":null},{\"ComercialId\":67,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Reuter Reuter\",\"EmpleadorACargo\":null},{\"ComercialId\":77,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Roberto Teijeiro\",\"EmpleadorACargo\":null},{\"ComercialId\":6,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Rodolfo Rene Chaulet \",\"EmpleadorACargo\":null},{\"ComercialId\":94,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Santiago Barbarotta\",\"EmpleadorACargo\":null},{\"ComercialId\":12,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Santiago Manfredi \",\"EmpleadorACargo\":null},{\"ComercialId\":32,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Sebastian Ulises Fiore U.\",\"EmpleadorACargo\":null},{\"ComercialId\":29,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Sergio Fabian Lisouski\",\"EmpleadorACargo\":null},{\"ComercialId\":26,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Sergio Martin Medina\",\"EmpleadorACargo\":null},{\"ComercialId\":71,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Soledad Remolins\",\"EmpleadorACargo\":null},{\"ComercialId\":69,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Soledad Romero\",\"EmpleadorACargo\":null},{\"ComercialId\":64,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"SRL Accenture\",\"EmpleadorACargo\":null},{\"ComercialId\":70,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Trinidad Dorna\",\"EmpleadorACargo\":null},{\"ComercialId\":90,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Verónica Bastidas\",\"EmpleadorACargo\":null},{\"ComercialId\":118,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":\"Yindri Bastidas\",\"EmpleadorACargo\":null}],\"campaña\":[{\"CampañaId\":1,\"Descripcion\":\"11-12\"},{\"CampañaId\":2,\"Descripcion\":\"12-13\"},{\"CampañaId\":3,\"Descripcion\":\"14-15\"},{\"CampañaId\":4,\"Descripcion\":\"15-16\"},{\"CampañaId\":5,\"Descripcion\":\"16-17\"},{\"CampañaId\":6,\"Descripcion\":\"17-18\"},{\"CampañaId\":7,\"Descripcion\":\"18-19\"},{\"CampañaId\":8,\"Descripcion\":\"19-20\"},{\"CampañaId\":9,\"Descripcion\":\"20-21\"}],\"proveedor\":null,\"monedaSustentable\":[{\"MonedaId\":\"ARP  \",\"Descripcion\":\"ARP\"},{\"MonedaId\":\"USDM \",\"Descripcion\":\"USD\"}],\"estadoContrato\":[{\"EstadosContratosId\":1,\"Descripcion\":\"Pendiente\"},{\"EstadosContratosId\":2,\"Descripcion\":\"Confirmado\"},{\"EstadosContratosId\":3,\"Descripcion\":\"Oferta\"},{\"EstadosContratosId\":4,\"Descripcion\":\"Con_Error\"},{\"EstadosContratosId\":5,\"Descripcion\":\"Finalizado\"},{\"EstadosContratosId\":6,\"Descripcion\":\"Rechazado\"},{\"EstadosContratosId\":7,\"Descripcion\":\"Reconfirmar\"},{\"EstadosContratosId\":8,\"Descripcion\":\"Eliminado\"},{\"EstadosContratosId\":9,\"Descripcion\":\"PreAprobacion\"},{\"EstadosContratosId\":10,\"Descripcion\":\"PreAnulado\"},{\"EstadosContratosId\":11,\"Descripcion\":\"ReconfirmarFinalizado\"}],\"Clasificacion\":[{\"Id\":1,\"Descripcion\":\"Productor\"},{\"Id\":2,\"Descripcion\":\"Acopiador\"},{\"Id\":3,\"Descripcion\":\"Otros\"}],\"Bolsa\":[{\"Id\":1,\"Descripcion\":\"Bs As\"},{\"Id\":2,\"Descripcion\":\"Rosario\"},{\"Id\":3,\"Descripcion\":\"Bahía Blanca\"},{\"Id\":4,\"Descripcion\":\"Santa Fe\"},{\"Id\":5,\"Descripcion\":\"Cordoba\"},{\"Id\":6,\"Descripcion\":\"Entre Ríos\"},{\"Id\":7,\"Descripcion\":\"Cordoba\"},{\"Id\":8,\"Descripcion\":\"Entre R?os\"},{\"Id\":9,\"Descripcion\":\"Bah?a Blanca\"}],\"Destino\":[{\"Id\":1,\"Descripcion\":\"S. Lorenzo\",\"CodigoSap\":null},{\"Id\":2,\"Descripcion\":\"Pergamino\",\"CodigoSap\":null},{\"Id\":3,\"Descripcion\":\"Bandera\",\"CodigoSap\":null},{\"Id\":4,\"Descripcion\":\"La Cautiva\",\"CodigoSap\":null},{\"Id\":5,\"Descripcion\":\"Lincoln\",\"CodigoSap\":null},{\"Id\":6,\"Descripcion\":\"Prest Dev. Buenos Aires\",\"CodigoSap\":null},{\"Id\":7,\"Descripcion\":\"Prest Dev. Santa Fe\",\"CodigoSap\":null},{\"Id\":8,\"Descripcion\":\"Chivilcoy\",\"CodigoSap\":null},{\"Id\":9,\"Descripcion\":\"LE\",\"CodigoSap\":null},{\"Id\":10,\"Descripcion\":\"SAN LORENZO SUSTENTABLE / CALIDAD\",\"CodigoSap\":null},{\"Id\":11,\"Descripcion\":\"Rio del Valle\",\"CodigoSap\":null},{\"Id\":12,\"Descripcion\":\"General Pinedo\",\"CodigoSap\":null}],\"Condicion\":[{\"Id\":1,\"Descripcion\":\"HASTA 12 HS. POR PIZARRA CIEGA\",\"CodigoSap\":null},{\"Id\":2,\"Descripcion\":\"HASTA 13 HS. POR PIZARRA CIEGA\",\"CodigoSap\":null},{\"Id\":3,\"Descripcion\":\"H 1/2 HORA AP CBOT X PIZ Ó MOA\",\"CodigoSap\":null},{\"Id\":4,\"Descripcion\":\"HASTA 12 HS. X PIZ.CIEGA Ó MOA\",\"CodigoSap\":null},{\"Id\":5,\"Descripcion\":\"HASTA 13 HS. X PIZ.CIEGA Ó MOA\",\"CodigoSap\":null},{\"Id\":6,\"Descripcion\":\"HASTA 14.30 HS POR PIZ / MERCADERIA\",\"CodigoSap\":null},{\"Id\":7,\"Descripcion\":\"MERCADO MOA\",\"CodigoSap\":null},{\"Id\":8,\"Descripcion\":\"H 1/2 HORA AP CBOT X PIZ ? MOA\",\"CodigoSap\":null},{\"Id\":9,\"Descripcion\":\"HASTA 12 HS. X PIZ.CIEGA ? MOA\",\"CodigoSap\":null},{\"Id\":10,\"Descripcion\":\"HASTA 13 HS. X PIZ.CIEGA ? MOA\",\"CodigoSap\":null}],\"Standard\":[{\"Id\":1,\"Descripcion\":\"Camara\",\"CodigoSap\":null},{\"Id\":2,\"Descripcion\":\"Especial\",\"CodigoSap\":null},{\"Id\":3,\"Descripcion\":\"Fabrica\",\"CodigoSap\":null},{\"Id\":4,\"Descripcion\":\"Camara\",\"CodigoSap\":null},{\"Id\":5,\"Descripcion\":\"Camara\",\"CodigoSap\":null},{\"Id\":6,\"Descripcion\":\"Bonif. SECO de 7% a 10% Por punto\",\"CodigoSap\":null},{\"Id\":7,\"Descripcion\":\"Grado 2\",\"CodigoSap\":null}],\"TipoDB\":[{\"Id\":1,\"Descripcion\":\"Sobre el precio\",\"CodigoSap\":null},{\"Id\":2,\"Descripcion\":\"Por Fuera del Precio\",\"CodigoSap\":null}],\"TipoPeriodoDB\":[{\"Id\":1,\"Descripcion\":\"Generales\",\"CodigoSap\":null},{\"Id\":2,\"Descripcion\":\"Por Fecha de Entrega\",\"CodigoSap\":null},{\"Id\":3,\"Descripcion\":\"Por Fecha de Fijación\",\"CodigoSap\":null}],\"MonedaDescuento\":[{\"MonedaId\":\"ARP  \",\"Descripcion\":\"ARP\"},{\"MonedaId\":\"USDM \",\"Descripcion\":\"USD\"}],\"TipoFason\":[{\"Id\":1,\"Descripcion\":\"FAS\"},{\"Id\":2,\"Descripcion\":\"FOB\"}],\"TipoAgenteCompra\":[{\"Id\":1,\"Descripcion\":\"MAT\"},{\"Id\":2,\"Descripcion\":\"Rofex\"}],\"Operador\":[{\"Id\":1,\"Descripcion\":\"Grain Solutions\"},{\"Id\":2,\"Descripcion\":\"Opción Agro\"}],\"Zona\":[{\"Id\":1,\"Descripcion\":\"0 USDM\"}],\"NivelTarifa\":[{\"Id\":1,\"Descripcion\":\"Estimado provisorio\",\"CodigoSap\":\"Z01\"},{\"Id\":2,\"Descripcion\":\"Estimado confirmado\",\"CodigoSap\":\"Z02\"},{\"Id\":3,\"Descripcion\":\"Real\",\"CodigoSap\":\"Z03\"}]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false}";
                return item.Replace("ñ", "ni");
            }
            try
            {
                var urlBusquedaMateriales = string.Concat(DataAgroURL, "/Compranet/InicializarContrato");

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(urlBusquedaMateriales, null);

                    task.Wait();

                    var stringContent = task.Result.Content.ReadAsStringAsync();

                    string scapedJson = stringContent.Result.Replace("ñ", "ni");

                    return stringContent.Result;

                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string ObtenerDatosCompraNet(int proveedorId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/ObtenerDatosCompraNet");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNet/ObtenerDatosCompraNet";
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { id = proveedorId });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string CrearContratoAFijar(ContratoAFijar contratoAPrecio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarContratoAFijar");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/GrabarContratoAFijar";
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(contratoAPrecio);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string GetContratos(DataSourceRequest request)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Contrato/BuscaDatosTabla");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/Contrato/BuscaDatosTabla";
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
