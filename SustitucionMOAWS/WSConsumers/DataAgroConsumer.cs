using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class DataAgroConsumer
    {
        private readonly DataAgroServicesClient service = new DataAgroServicesClient();

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

        public ResultadoAltaCampoSustentable AltaCampoSustentable(SustitucionMOAModel.Entities.CampoProveedor campo, string kmz)
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

        public InicializarContratoDto InicializarContrato(int tipoNegocioId)
        {
            var result = service.InicializarContrato(tipoNegocioId);
            return result;
        }

        public KendoGridResponseDtoOfConfiguracionBolsaDtocyovIo6p ConfiguracionBolsaAutomatica()
        {
            var configuracion = service.ObtenerConfiguracionBolsa();
            return configuracion;
        }

        public DatosCompraNetDto ObtenerDatosCompraNet(int id)
        {
            var result = service.ObtenerDatosCompraNet(id);
            return result;
        }

        public DatosFijacionDeContratoDto[] ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual)
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

        public SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro AnularFijacion(int negocioId, string motivoRechazo)
        {
            var result = service.AnularFijacion(negocioId, motivoRechazo);
            SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro resultado = new SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro();
            resultado.Errores = result.ListaErrores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessage { Message = a.Message }).ToList();
            return resultado;
        }

        public SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro AnularContrato(int negocioId, string motivoRechazo)
        {
            var result = service.AnularContrato(negocioId, motivoRechazo);
            SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro resultado = new SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro();
            resultado.Errores = result.ListaErrores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessage { Message = a.Message }).ToList();
            return resultado;
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

        public BuscarMaterialesDto BuscarMateriales()
        {
            var result = service.BuscarMateriales();
            return result;
        }

        public SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto GrabarContratoAPrecio(Contrato contrato)
        {
            var grabarContratoResult = service.GrabarContratoAPrecio(contrato);
            SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto resultDto = new SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto();
            resultDto.ContratoId = grabarContratoResult.ContratoId;
            resultDto.Errores = grabarContratoResult.Errores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessageDtos { Message = a }).ToList();
            return resultDto;
        }

        public SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto GrabarContratoAFijar(Contrato contrato)
        {
            contrato.Pizarra = contrato.Pizarra ?? false;
            var grabarContratoResult = service.GrabarContratoAFijar(contrato);
            SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto resultDto = new SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto();
            resultDto.ContratoId = grabarContratoResult.ContratoId;
            resultDto.Errores = grabarContratoResult.Errores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessageDtos { Message = a }).ToList();
            return resultDto;
        }

        public SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto GrabarFijacion(FijacionDePrecioContrato contrato)
        {
            var result = service.GrabarFijacion(contrato);
            SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto resultDto = new SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto();
            resultDto.FijacionDePrecioContratoId = result.FijacionDePrecioContratoId;
            resultDto.Errores = result.Errores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessageDtos { Message = a }).ToList();
            return resultDto;
        }

        public ContratoCopiar[] TraerContratosAcuerdoPorCorredor(int corredorId)
        {
            var result = service.TraerContratosAcuerdoPorCorredor(corredorId);
            return result;
        }

        public SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto[] GrabarContratoMasivo(BasicoContrato[] contratos)
        {
            var result = service.GrabarContratoMasivo(contratos);
            List<SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto> resultDto = new List<SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto>();

            foreach (var resultado in result)
            {

                SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto dto = new SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto
                {
                    FijacionDePrecioContratoId = resultado.FijacionDePrecioContratoId,
                    ContratoId = resultado.ContratoId,
                    Errores = resultado.Errores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessageDtos { Message = a, Source = "" }).ToList()
                };
                resultDto.Add(dto);
            }
            return resultDto.ToArray();
        }

        public BasicoContrato TraerContratoCompleto(int id, string tipo)
        {
            var result = service.TraerContratoCompleto(id, tipo);
            return result;
        }

        public BasicoContrato TraerFijacionCompleto(int id)
        {
            var result = service.TraerFijacionCompleto(id);
            return result;
        }


        public bool ValidarDirecto(string cuit)
        {
            var esValido = service.ValidarDirecto(cuit);
            return esValido;
        }

        public Byte[] ExcelModeloAltaMasiva()
        {
            var result = service.ExcelModeloAltaMasiva();
            return result;
        }

        public BuscarCentroDto BuscarCentro()
        {
            var resultIniCentro = service.BuscarCentro();
            return resultIniCentro;
        }

        public CampañaDto[] BuscarCampanas()
        {
            var campanasDto = service.BuscarCampana();
            return campanasDto;
        }

        public LocalidadDto[] ListarLocalidades()
        {
            var localidades = service.ListarLocalidades();
            return localidades;
        }

        public PartidoDto[] ListarPartidos()
        {
            var partidos = service.ListarPartidos();
            return partidos;
        }

        public KendoDataSourceResultDto BuscaDatosTablaContrato(KendoDataSourceRequestDto filtro)
        {
            var resultDto = service.BuscaDatosTablaContrato(filtro);
            return resultDto;
        }

        public ListarFeriadosDto ListarFeriados()
        {
            var feriados = service.ListarFeriados();
            return feriados;
        }

        public RespuestaArchivoDto CamposSustentables(DeclaracionCampoSustentable datos)
        {
            var respuesta = service.CamposSustentables(datos);
            return respuesta;
        }
    }
}
