using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class DataAgroConsumer
    {
        private DataAgroServicesClient service = new DataAgroServicesClient();

        public DataAgroConsumer()
        {
            service.ClientCredentials.UserName.UserName = string.Concat(DataAgroWSCredential.getDominio(), @"\", DataAgroWSCredential.getUserName());
            service.ClientCredentials.UserName.Password = DataAgroWSCredential.getPassword();
            //service.ClientCredentials.UserName. = DataAgroWSCredential.getDominio();
        }


        public ResultadoValidarProveedorComercial ValidarCUIT(string CUIT, bool? corredor = false)
        {
            try
            {
                return service.ValidarProveedorComercial(CUIT, corredor);
            }
            //Significa que no estamos conectados
            catch (Exception)
            {
                return null;
            }
        }

        public bool ProveedorApocrifo(string CUIT)
        {
            try
            {
                return service.ProveedorApocrifo(CUIT);
            }
            //Significa que no estamos conectados
            catch (Exception)
            {
                return false;
            }
        }

        public decimal TraerTipoDeCambio()
        {
            return service.TraerTipoDeCambio(DateTime.Now.Date, "", "M");
        }

        public SustitucionMOAWS.DataAgroServices.ResultadoAltaCampoSustentable AltaCampoSustentable(SustitucionMOAModel.Entities.CampoProveedor campo, string kmz)
        {

            return service.AltaCampoSustentable(new CampoDetalleTerceroDto
            {
                ProveedorCUIT = campo.CUIT,
                Campania = campo.CampoCosecha.Cosecha.Nombre,
                LocalidadId = campo.CampoCosecha.Campo.Localidad_Id,
                Latitud = campo.Latitud,
                Longitud = campo.Longitud,
                KMZnombre = campo.Archivo.FileKey,
                KMZfileBase64 = kmz.Substring(400),
                Nombre = campo.CampoCosecha.Campo.Nombre,
                ToneladasAprobadas = Convert.ToDecimal(campo.CampoCosecha.CampoCosechaNormativas.FirstOrDefault().ToneladasAprobadas),
                HectareasTotales = Convert.ToDecimal(campo.HectareasTotales),
                HectareasCultivables = Convert.ToDecimal(campo.HectareasSoja),
                Id = campo.CampoCosecha.Id,
                Estado = campo.Proveedor.EstadoAprobacion.ToString()
            });
        }

        public ResultEstadoProveedores ObtenerEstadoProveedores(string[] cuits)
        {
            var result = service.ObtenerEstadoProveedores(cuits);
            foreach (var item in result.Contactos.Where(a => a.OperaConMATBA))
            {
                item.EstadoHomeDescripcion = "No Habilitado";
            }
            return result;

        }

        public ContratoModel_prueba InicializarContrato(int tipoNegocioId)
        {
            var result = service.InicializarContrato(tipoNegocioId);
            return result; 
        }

        public DatosCompraNetDto ObtenerDatosCompraNet(int id)
        {
            var result = service.ObtenerDatosCompraNet(id);
            return result;
        }

        public SustitucionMOAWS.DataAgroServices.DatosFijacionDeContratoDto[] ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual)
        {
            var result = service.ObtenerFijacionesAutomaticas(cuitProveedor, cuitCorredor, materialId, filtro, fijacionId, esVirtual);
            return result;
        }

        public AltaTempranaNRCODto ValidarProveedor(int proveedorId)
        {
            var result = service.ValidarProveedor(proveedorId);
            return result;
        }

        public HabilitacionPizarraDto HabilitarPizarra(int material, int tipoNegocio)
        {
            var result = service.HabilitarPizarra(material, tipoNegocio);
            return result;
        }

        public HabilitacionPagoDiferidoDto[] TraerPagosDiferido()
        {
            var result = service.TraerPagosDiferido();
            return result;
        }

        public HabilitacionCampañaDto[] HabilitarCampaña(int material)
        {
            var result = service.HabilitarCampaña(material);
            return result;
        }

        public PrecioMoaCompraNetDto[][] TraerPrecioMOA(int tipoNegocio)
        {
            var result = service.TraerPrecioMoa(tipoNegocio);
            return result;
        }

        public PrecioMoaCompraNetDto[] TraerPrecioMoaV2(int material, int tipoNegocio)
        {
            var result = service.TraerPrecioMoaV2(material, tipoNegocio);
            return result;
        }

        public Resultado AnularFijacion(int negocioId, string motivoRechazo)
        {
            var result = service.AnularFijacion(negocioId, motivoRechazo);
            return result;
        }

        public Resultado AnularContrato(int negocioId, string motivoRechazo)
        {
            var result = service.AnularContrato(negocioId, motivoRechazo);
            return result;
        }

        public HabilitacionSustentableDto[] HabilitarSustentable()
        {
            var result = service.HabilitarSustentable();
            return result;
        }

        public BusquedaHome[] BuscarProveedoresConCorredor(string filtroProveedor, string filtro, int? agenteCompraId)
        {
            var result = service.BuscarProveedoresConCorredor(filtroProveedor, filtro, agenteCompraId);
            return result;
        }

        public ResultIniMaterialModel BuscarMateriales()
        {
            var result = service.BuscarMateriales();
            return result;
        }

        public DataAgroServices.GrabarContratoResult GrabarContratoAPrecio(Contrato contrato)
        {
            var grabarContratoResult = service.GrabarContratoAPrecio(contrato);
            return grabarContratoResult;
        }

        public DataAgroServices.GrabarContratoResult GrabarContratoAFijar(Contrato contrato)
        {
            var grabarContratoResult = service.GrabarContratoAFijar(contrato);
            return grabarContratoResult;
        }

        public GrabarFijacionResult GrabarFijacion(FijacionDePrecioContrato contrato)
        {
            var result = service.GrabarFijacion(contrato);
            return result;
        }

        public ContratoCopiar[] TraerContratosAcuerdoPorCorredor(int corredorId)
        {
            var result = service.TraerContratosAcuerdoPorCorredor(corredorId);
            return result;
        }

        public DataAgroServices.GrabarContratoResult[] GrabarContratoMasivo(DataAgroServices.BasicoContrato[] contratos)
        {
            var result = service.GrabarContratoMasivo(contratos);
            return result;
        }
        public DataAgroServices.BasicoContrato TraerContratoCompleto(int id, string tipo)
        {
            var result = service.TraerContratoCompleto(id, tipo);
            return result;
        }

        public DataAgroServices.BasicoContrato TraerFijacionCompleto(int id)
        {
            var result = service.TraerFijacionCompleto(id);
            return result;
        }


        public bool ValidarDirecto(string cuit)
        {
            var esValido = service.ValidarDirecto(cuit);
            return esValido;
        }
    }
}
