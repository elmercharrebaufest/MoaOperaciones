using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using SustitucionMOAWS.AplicacionesWebServiceMOA;
using SustitucionMOAWS.CambioPassWebServiceMOA;
using SustitucionMOAWS.CartaPorteDetalleWebServiceMOA;
using SustitucionMOAWS.CartaPorteFormularioCTGWebServiceMOA;
using SustitucionMOAWS.CartaPorteFormularioDesplegablesWebServiceMOA;
using SustitucionMOAWS.ComprobantesNGWebServiceMOA;
using SustitucionMOAWS.ContactoMailCategoriasWebServiceMOA;
using SustitucionMOAWS.ContactoMailWebServiceMOA;
using SustitucionMOAWS.ContratoDetalleWebServiceMOA;
using SustitucionMOAWS.ContratoPDFWebServiceMOA;
using SustitucionMOAWS.ContratosWebServiceMOA;
using SustitucionMOAWS.CrearPedidoWebServiceMOA;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.CrearUsuarioWebServiceMOA;
using SustitucionMOAWS.CuentaCorrienteWebServiceMOA;
using SustitucionMOAWS.DetalleCteWebServiceMOA;
using SustitucionMOAWS.EcheqAnularAperturaChequeWebServiceMOA;
using SustitucionMOAWS.EcheqCargaAperturaChequeWebServiceMOA;
using SustitucionMOAWS.EcheqModificacionDocumentoChequeWebServiceMOA;
using SustitucionMOAWS.EcheqModificarContratoWebServiceMOA;
using SustitucionMOAWS.EcheqModificarFijacionWebServiceMOA;
using SustitucionMOAWS.EcheqVisualizarDisponibleChequeWebServiceMOA;
using SustitucionMOAWS.EcheqVisualizarPendientePagoWebServiceMOA;
using SustitucionMOAWS.FijacionDetalleWebServiceMOA;
using SustitucionMOAWS.FijacionesWebServiceMOA;
using SustitucionMOAWS.FletesRelacionWebServiceMOA;
using SustitucionMOAWS.FletesWebServiceMOA;
using SustitucionMOAWS.FleteValidarImporteWebServiceMOA;
using SustitucionMOAWS.HomeNGWebServiceMOA;
using SustitucionMOAWS.HomeWebServiceMOA;
using SustitucionMOAWS.LiquidacionesNGWebServiceMOA;
using SustitucionMOAWS.LiquidacionesWebServiceMOA;
using SustitucionMOAWS.ListarPesificaciones;
using SustitucionMOAWS.LoginWebServiceMOA;
using SustitucionMOAWS.ModificarEntregaOrdenFasWebServiceMOA;
using SustitucionMOAWS.ModificarOrdenCargaFasWebServiceMOA;
using SustitucionMOAWS.ModificarSolpWebServiceMOA;
using SustitucionMOAWS.MovimientoBalanzaWebServiceMOA;
using SustitucionMOAWS.NoticiasDetalleWebServiceMOA;
using SustitucionMOAWS.ObtenerCecoSolpWebServiceMOA;
using SustitucionMOAWS.ObtenerCuentasSolpWebServiceMOA;
using SustitucionMOAWS.ObtenerMaterialesSolpWebServiceMOA;
using SustitucionMOAWS.ObtenerOrdenDeCompraWebServiceMOA;
using SustitucionMOAWS.ObtenerOrdenSolpWebServiceMOA;
using SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA;
using SustitucionMOAWS.ObtenerServiciosSolpWebServiceMOA;
using SustitucionMOAWS.ObtenerSolpWebServiceMOA;
using SustitucionMOAWS.OrdenCargaControlEstadoSAP;
using SustitucionMOAWS.OrdenCargaControlSAP;
using SustitucionMOAWS.OrdenCargaCrearSAP;
using SustitucionMOAWS.OrdenCargaEstadoEntregadaSAP;
using SustitucionMOAWS.OrdenCargaVisualizarCliente;
using SustitucionMOAWS.PagoComprobantesWebServiceMOA;
using SustitucionMOAWS.PagoDetalleWebServiceMOA;
using SustitucionMOAWS.PagosNGWebServiceMOA;
using SustitucionMOAWS.PagosWebServiceMOA;
using SustitucionMOAWS.PDFComprobantesNGWebServiceMOA;
using SustitucionMOAWS.PDFProformaFinalWebServiceMOA;
using SustitucionMOAWS.PDFWebServiceMOA;
using SustitucionMOAWS.PerfilesWebServiceMOA;
using SustitucionMOAWS.PermisosWebServiceMOA;
using SustitucionMOAWS.PesificacionGuadarWebServiceMOA;
using SustitucionMOAWS.PesificacionWebServiceMOA;
using SustitucionMOAWS.ProformaFleteProcedenciaServiceMOA;
using SustitucionMOAWS.RecepcionesWebServiceMOA;
using SustitucionMOAWS.UsuarioDesbloquearWebServiceMOA;
using SustitucionMOAWS.UsuarioHabilitarWebServiceMOA;
using SustitucionMOAWS.UsuarioInhabilitarWebServiceMOA;
using SustitucionMOAWS.UsuarioNuevoWebServiceMOA;
using SustitucionMOAWS.UsuarioOlvidePassWebServiceMOA;
using SustitucionMOAWS.UsuariosWebServiceMOA;
using SustitucionMOAWS.VendedorDetalleWebServiceMOA;
using SustitucionMOAWS.VendedoresWebServiceMOA;
using SustitucionMOAWS.VendedorHabilitadoWebServiceMOA;
using SustitucionMOAWS.VinculaDetalleWebServiceMOA;

namespace SustitucionMOATest.DeployParameters
{
    [TestFixture]
    public class EndpointTests
    {
        [Test]
        public void VerifyAllWebConfigEndpointsAreInProdParameters()
        {
            string webProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SustitucionMOA\");
            string webConfigPath = Path.Combine(webProjectPath, "web.config");
            string webProdFilePathPath = Path.Combine(webProjectPath, "NEW.PROD.DeployParameters.xml");

            // Extraer los endpoints de ambos archivos
            var webConfigEndpoints = ExtractEndpointsFromWebConfig(webConfigPath);
            var prodEndpoints = ExtractEndpointsFromProd(webProdFilePathPath);

            // Verificar en bloque múltiple
            Assert.Multiple(() =>
            {
                // Verificar si faltan en el archivo de producción
                var faltanEnElProd = webConfigEndpoints.Except(prodEndpoints).ToList();
                Assert.IsEmpty(faltanEnElProd, $"Faltan en el NEW.PROD.DeployParameters.xml: {string.Join(", ", faltanEnElProd)}");

                // Verificar si faltan en el web.config
                var faltanEnElWebConfig = prodEndpoints.Except(webConfigEndpoints).ToList();
                Assert.IsEmpty(faltanEnElWebConfig, $"Faltan en el web.config: {string.Join(", ", faltanEnElWebConfig)}");
            });
        }

        [Test]
        public void VerifyAllWebConfigEndpointsAreInParametersFile()
        {
            string webProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SustitucionMOA\");
            string parametersFilePath = Path.Combine(webProjectPath, "parameters.xml");
            string webConfigPath = Path.Combine(webProjectPath, "web.config");

            // Extraer los endpoints de ambos archivos
            var parametersEndpoints = ExtractEndpointsFromParameters(parametersFilePath);
            var webConfigEndpoints = ExtractEndpointsFromWebConfig(webConfigPath);

            // Verificar en bloque múltiple
            Assert.Multiple(() =>
            {
                // Verificar si faltan en el archivo de producción
                var faltanEnElProd = webConfigEndpoints.Except(parametersEndpoints).ToList();
                Assert.IsEmpty(faltanEnElProd, $"Faltan en el parameters.xml: {string.Join(", ", faltanEnElProd)}");

                // Verificar si faltan en el web.config
                var faltanEnElWebConfig = parametersEndpoints.Except(webConfigEndpoints).ToList();
                Assert.IsEmpty(faltanEnElWebConfig, $"Faltan en el web.config: {string.Join(", ", faltanEnElWebConfig)}");
            });
        }

        [Test]
        public void VerifyAllWebConfigEndpointsAreInProdParameters_ExternalAPI()
        {
            string webProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SustitucionMOAExternalAPI\");
            string webConfigPath = Path.Combine(webProjectPath, "web.config");
            string webProdFilePathPath = Path.Combine(webProjectPath, "NEW.PROD.DeployParameters.xml");

            var webConfigEndpoints = ExtractEndpointsFromWebConfig(webConfigPath);
            var prodEndpoints = ExtractEndpointsFromProd(webProdFilePathPath);

            Assert.Multiple(() =>
            {
                var faltanEnElProd = webConfigEndpoints.Except(prodEndpoints).ToList();
                Assert.IsEmpty(faltanEnElProd, $"Faltan en el NEW.PROD.DeployParameters.xml: {string.Join(", ", faltanEnElProd)}");

                var faltanEnElWebConfig = prodEndpoints.Except(webConfigEndpoints).ToList();
                Assert.IsEmpty(faltanEnElWebConfig, $"Faltan en el web.config: {string.Join(", ", faltanEnElWebConfig)}");
            });
        }

        [Test]
        public void VerifyAllWebConfigEndpointsAreInParametersFile_ExternalAPI()
        {
            string webProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SustitucionMOAExternalAPI\");
            string parametersFilePath = Path.Combine(webProjectPath, "parameters.xml");
            string webConfigPath = Path.Combine(webProjectPath, "web.config");

            var parametersEndpoints = ExtractEndpointsFromParameters(parametersFilePath);
            var webConfigEndpoints = ExtractEndpointsFromWebConfig(webConfigPath);

            Assert.Multiple(() =>
            {
                var faltanEnElProd = webConfigEndpoints.Except(parametersEndpoints).ToList();
                Assert.IsEmpty(faltanEnElProd, $"Faltan en el parameters.xml: {string.Join(", ", faltanEnElProd)}");

                var faltanEnElWebConfig = parametersEndpoints.Except(webConfigEndpoints).ToList();
                Assert.IsEmpty(faltanEnElWebConfig, $"Faltan en el web.config: {string.Join(", ", faltanEnElWebConfig)}");
            });
        }

        private List<string> ExtractEndpointsFromParameters(string filePath)
        {
            var parametersXml = XDocument.Load(filePath);
            var endpoints = parametersXml.Descendants("parameter")
                                         .Where(x => x.Attribute("name")?.Value.Contains("Service Endpoint Address") == true)
                                         .Select(x => x.Attribute("name")?.Value.Split(' ')[0]) // Extraer solo el nombre del endpoint
                                         .ToList();
            return endpoints;
        }

        private List<string> ExtractEndpointsFromWebConfig(string filePath)
        {
            var webConfigXml = XDocument.Load(filePath);
            var endpoints = webConfigXml.Descendants("endpoint")
                                        .Select(x => x.Attribute("name")?.Value)
                                        .Where(name => !string.IsNullOrEmpty(name))
                                        .ToList();
            return endpoints;
        }

        private List<string> ExtractEndpointsFromProd(string filePath)
        {
            var prodXml = XDocument.Load(filePath);
            var endpoints = prodXml.Descendants("setParameter")
                                   .Where(x => x.Attribute("name")?.Value.Contains("Service Endpoint Address") == true)
                                   .Select(x => x.Attribute("name")?.Value.Split(' ')[0])  // Para obtener solo el nombre del endpoint
                                   .ToList();
            return endpoints;
        }
    }
}
