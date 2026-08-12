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
        private readonly DataAgroServicesFullClient serviceFull = new DataAgroServicesFullClient();

        public DataAgroConsumer()
        {
            service.ClientCredentials.UserName.UserName = string.Concat(DataAgroWSCredential.getDominio(), @"\", DataAgroWSCredential.getUserName());
            service.ClientCredentials.UserName.Password = DataAgroWSCredential.getPassword();
            serviceFull.ClientCredentials.UserName.UserName = string.Concat(DataAgroWSCredential.getDominio(), @"\", DataAgroWSCredential.getUserName());
            serviceFull.ClientCredentials.UserName.Password = DataAgroWSCredential.getPassword();
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

        public ResultadoValidarProveedorComercial ValidarCUITNuevo(string CUIT, bool? corredor = false, string cuitCorredor = "")
        {
            try
            {
                return serviceFull.ValidarProveedorComercialNuevo(CUIT, corredor, cuitCorredor);
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
            var result = serviceFull.ObtenerEstadoProveedores(cuits);
            foreach (var item in result.Contactos.Where(a => a.OperaConMATBA))
            {
                item.EstadoHomeDescripcion = "No Habilitado";
            }
            return result;

        }
        
        public InicializarContratoDto InicializarContrato(int tipoNegocioId)
        {
            var result = serviceFull.InicializarContrato(tipoNegocioId);
            return result;
        }

        public KendoGridResponseDtoOfConfiguracionBolsaDtocyovIo6p ConfiguracionBolsaAutomatica()
        {
            var configuracion = serviceFull.ObtenerConfiguracionBolsa();
            return configuracion;
        }
        
        public DatosCompraNetDto ObtenerDatosCompraNet(int id)
        {
            var result = serviceFull.ObtenerDatosCompraNet(id);
            return result;
        }

        public DatosFijacionDeContratoDto[] ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual)
        {
            var result = serviceFull.ObtenerFijacionesAutomaticas(cuitProveedor, cuitCorredor, materialId, filtro, fijacionId, esVirtual);
            return result;
        }
        
        public AltaTempranaNRCODto ValidarProveedor(int proveedorId)
        {
            var result = serviceFull.ValidarProveedor(proveedorId);
            return result;
        }

        public HabilitacionPizarraDto HabilitarPizarra(int material, int tipoNegocio)
        {
            var result = serviceFull.HabilitarPizarra(material, tipoNegocio);
            return result;
        }

        public HabilitacionPagoDiferidoDto[] TraerPagosDiferido()
        {
            var result = serviceFull.TraerPagosDiferido();
            return result;
        }

        public HabilitacionCampañaDto[] HabilitarCampaña(int material)
        {
            var result = serviceFull.HabilitarCampaña(material);
            return result;
        }

        public PrecioMoaCompraNetDto[][] TraerPrecioMOA(int tipoNegocio)
        {
            var result = serviceFull.TraerPrecioMoa(tipoNegocio);
            return result;
        }

        public PrecioMoaCompraNetDto[] TraerPrecioMoaV2(int material, int tipoNegocio)
        {
            var result = serviceFull.TraerPrecioMoaV2(material, tipoNegocio);
            return result;
        }

        public SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro AnularFijacion(int negocioId, string motivoRechazo)
        {
            var result = serviceFull.AnularFijacion(negocioId, motivoRechazo);
            SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro resultado = new SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro();
            resultado.Errores = result.ListaErrores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessage { Message = a.Message }).ToList();
            return resultado;
        }

        public SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro AnularContrato(int negocioId, string motivoRechazo)
        {
            var result = serviceFull.AnularContrato(negocioId, motivoRechazo);
            SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro resultado = new SustitucionMOAModel.Models.DataAgro.ResultadoDataAgro();
            resultado.Errores = result.ListaErrores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessage { Message = a.Message }).ToList();
            return resultado;
        }

        public HabilitacionSustentableDto[] HabilitarSustentable()
        {
            var result = serviceFull.HabilitarSustentable();
            return result;
        }

        public BusquedaHome[] BuscarProveedoresConCorredor(string filtroProveedor, string filtro, int? agenteCompraId)
        {
            var result = serviceFull.BuscarProveedoresConCorredor(filtroProveedor, filtro, agenteCompraId);
            return result;
        }

        public BuscarMaterialesDto BuscarMateriales()
        {
            var result = serviceFull.BuscarMateriales();
            return result;
        }

        public SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto GrabarContratoAPrecio(Contrato contrato)
        {
            var grabarContratoResult = serviceFull.GrabarContratoAPrecio(contrato);
            SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto resultDto = new SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto();
            resultDto.ContratoId = grabarContratoResult.ContratoId;
            resultDto.Errores = grabarContratoResult.Errores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessageDtos { Message = a }).ToList();
            return resultDto;
        }

        public SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto GrabarContratoAFijar(Contrato contrato)
        {
            contrato.Pizarra = contrato.Pizarra ?? false;
            var grabarContratoResult = serviceFull.GrabarContratoAFijar(contrato);
            SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto resultDto = new SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto();
            resultDto.ContratoId = grabarContratoResult.ContratoId;
            resultDto.Errores = grabarContratoResult.Errores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessageDtos { Message = a }).ToList();
            return resultDto;
        }

        public SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto GrabarFijacion(FijacionDePrecioContrato contrato)
        {
            var result = serviceFull.GrabarFijacion(contrato);
            SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto resultDto = new SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto();
            resultDto.FijacionDePrecioContratoId = result.FijacionDePrecioContratoId;
            resultDto.Errores = result.Errores.Select(a => new SustitucionMOAModel.Models.DataAgro.ErrorMessageDtos { Message = a }).ToList();
            return resultDto;
        }

        public ContratoCopiar[] TraerContratosAcuerdoPorCorredor(int corredorId)
        {
            var result = serviceFull.TraerContratosAcuerdoPorCorredor(corredorId);
            return result;
        }

        public SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto[] GrabarContratoMasivo(BasicoContrato[] contratos)
        {
            var result = serviceFull.GrabarContratoMasivo(contratos);
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
            var result = serviceFull.TraerContratoCompleto(id, tipo);
            return result;
        }

        public BasicoContrato TraerFijacionCompleto(int id)
        {
            var result = serviceFull.TraerFijacionCompleto(id);
            return result;
        }


        public bool ValidarDirecto(string cuit)
        {
            var esValido = serviceFull.ValidarDirecto(cuit);
            return esValido;
        }

        public Byte[] ExcelModeloAltaMasiva()
        {
            var result = serviceFull.ExcelModeloAltaMasiva();
            return result;
        }

        public BuscarCentroDto BuscarCentro()
        {
            var resultIniCentro = serviceFull.BuscarCentro();
            return resultIniCentro;
        }

        public CampañaDto[] BuscarCampanas()
        {
            var campanasDto = serviceFull.BuscarCampana();
            return campanasDto;
        }

        public LocalidadDto[] ListarLocalidades()
        {
            var localidades = serviceFull.ListarLocalidades();
            return localidades;
        }

        public PartidoDto[] ListarPartidos()
        {
            var partidos = serviceFull.ListarPartidos();
            return partidos;
        }

        public KendoDataSourceResultDto BuscaDatosTablaContrato(KendoDataSourceRequestDto filtro)
        {
            var resultDto = serviceFull.BuscaDatosTablaContrato(filtro);
            return resultDto;
        }

        public ListarFeriadosDto ListarFeriados()
        {
            var feriados = serviceFull.ListarFeriados();
            return feriados;
        }

        public RespuestaArchivoDto CamposSustentables(DeclaracionCampoSustentable datos)
        {
            var respuesta = serviceFull.CamposSustentables(datos);
            return respuesta;
        }

        public RespuestaArchivoDto FormularioAltaNoGranos(ProveedorAltaDto proveedorAlta)
        {
            var respuestaArchivo = serviceFull.FormularioAltaNoGranos(proveedorAlta);
            return respuestaArchivo;
        }

        public RespuestaArchivoDto CartaDePresentacion(RptCartaDePresentacionInfo oParam, NuevoProduccion[] nuevosCampos, NuevoAcopio[] nuevosAcopios)
        {
            var respuesta = serviceFull.CartaDePresentacion(oParam, nuevosCampos, nuevosAcopios);
            return respuesta;
        }

        public DataAgroServices.RespuestaArchivoDto ListarInformeComercial(DataAgroServices.ParamInformeComercial oParam, int? comercialId, DataAgroServices.NuevoProduccion[] nuevosCampos,
            DataAgroServices.NuevoAcopio[] nuevosAcopios, DataAgroServices.ContactoComercial contactoComercial, string direccion, string codigoPostal, int? localidadId)
        {
            var informe = serviceFull.InformeComercial(oParam, comercialId, nuevosCampos, nuevosAcopios, contactoComercial, direccion, codigoPostal, localidadId);
            return informe;
        }
    }
}
