using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

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

            // Verificar si todos los endpoints del web.config están en NEW.PROD.DeployParameters.xml
            var missingEndpoints = webConfigEndpoints.Except(prodEndpoints).ToList();

            if (missingEndpoints.Any())
            {
                Assert.Fail($"Faltan los siguientes endpoints en el archivo de producción: {string.Join(", ", missingEndpoints)}");
            }
            else
            {
                Assert.Pass("Todos los endpoints están presentes en el archivo de producción.");
            }
        }

        [Test]
        public void VerifyAllWebConfigEndpointsAreInProdParameters_ExternalAPI()
        {
            string webProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SustitucionMOAExternalAPI\");
            string webConfigPath = Path.Combine(webProjectPath, "web.config");
            string webProdFilePathPath = Path.Combine(webProjectPath, "NEW.PROD.DeployParameters.xml");

            // Extraer los endpoints de ambos archivos
            var webConfigEndpoints = ExtractEndpointsFromWebConfig(webConfigPath);
            var prodEndpoints = ExtractEndpointsFromProd(webProdFilePathPath);

            // Verificar si todos los endpoints del web.config están en NEW.PROD.DeployParameters.xml
            var missingEndpoints = webConfigEndpoints.Except(prodEndpoints).ToList();

            if (missingEndpoints.Any())
            {
                Assert.Fail($"Faltan los siguientes endpoints en el archivo de producción: {string.Join(", ", missingEndpoints)}");
            }
            else
            {
                Assert.Pass("Todos los endpoints están presentes en el archivo de producción.");
            }
        }

        [Test]
        public void VerifyAllWebConfigEndpointsAreInParametersFile()
        {
            string webProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SustitucionMOA\");
            string parametersFilePath = Path.Combine(webProjectPath, "parameters.xml");
            string webConfigPath = Path.Combine(webProjectPath, "web.config");

            // Extraer los endpoints de ambos archivos
            var parametersEndpoints = ExtractEndpointsFromParameters(parametersFilePath);
            var prodEndpoints = ExtractEndpointsFromWebConfig(webConfigPath);

            // Verificar si todos los endpoints de parameters.xml están en NEW.PROD.DeployParameters.xml
            var missingEndpoints = parametersEndpoints.Except(prodEndpoints).ToList();

            if (missingEndpoints.Any())
            {
                Assert.Fail($"Faltan los siguientes endpoints en el archivo de producción: {string.Join(", ", missingEndpoints)}");
            }
            else
            {
                Assert.Pass("Todos los endpoints están presentes en el archivo de producción.");
            }
        }

        [Test]
        public void VerifyAllWebConfigEndpointsAreInParametersFile_ExternalAPI()
        {
            string webProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SustitucionMOAExternalAPI\");
            string parametersFilePath = Path.Combine(webProjectPath, "parameters.xml");
            string webConfigPath = Path.Combine(webProjectPath, "web.config");

            // Extraer los endpoints de ambos archivos
            var parametersEndpoints = ExtractEndpointsFromParameters(parametersFilePath);
            var prodEndpoints = ExtractEndpointsFromWebConfig(webConfigPath);

            // Verificar si todos los endpoints de parameters.xml están en NEW.PROD.DeployParameters.xml
            var missingEndpoints = parametersEndpoints.Except(prodEndpoints).ToList();

            if (missingEndpoints.Any())
            {
                Assert.Fail($"Faltan los siguientes endpoints en el archivo de producción: {string.Join(", ", missingEndpoints)}");
            }
            else
            {
                Assert.Pass("Todos los endpoints están presentes en el archivo de producción.");
            }
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
