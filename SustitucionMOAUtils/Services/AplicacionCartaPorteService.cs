using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using System.ComponentModel.DataAnnotations;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class AplicacionCartaPorteService : IAplicacionCartaPorteService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IAplicacionCartaPorteConsumer consumer;
        public AplicacionCartaPorteService(IRepositorio repositorio, IAplicacionCartaPorteConsumer consumer)
        {
            this.repositorio = repositorio;
            this.consumer = consumer;
        }
        public List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin)
        {
            var fechaInicioDateTime = DataFormatter.StringToDateTime(fechaInicio, "fechaInicio");
            var fechaFinDateTime = DataFormatter.StringToDateTime(fechaFin, "fechaFin");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            string cuit = usuario.CUITRegistro;
            bool isAdmin = usuario.TienePermiso(PermisoEnum.AbmAplicacionesCcpp);

            var rawList = repositorio.Listar<AplicacionCartaPorte>(apl =>
                (isAdmin || apl.Proveedor.CUIT == cuit) &&
                fechaInicioDateTime <= apl.FechaAlta && fechaFinDateTime >= DbFunctions.TruncateTime(apl.FechaAlta) &&
                apl.Estado != EstadoAplicacionCartaPorte.Eliminado
            ) ;

            var lista = rawList.Select(apl => new AplicacionCartaPorteDto(apl)).ToList();
            return lista;
        }
        public AplicacionCartaPorteDto Obtener(int aplicacionCCPPId, string mailUsuario)
        {
            return new AplicacionCartaPorteDto(new AplicacionCartaPorte());
        }
        public AplicacionCartaPorteFiltrosDto ObtenerFiltros(List<AplicacionCartaPorteDto> aplicaciones)
        {
            return new AplicacionCartaPorteFiltrosDto(aplicaciones);
        }
        public void EliminarAplicacion(int aplicacionId)
        {
            var aplicacion = repositorio.Obtener<AplicacionCartaPorte>(aplicacionId);
            if(aplicacion == null)
                throw new InfoCustomException("No se ha encontrado la aplicación.");
            if (aplicacion.Estado != EstadoAplicacionCartaPorte.Pendiente)
                throw new InfoCustomException("No se puede eliminar la aplicación.");
            aplicacion.Estado = EstadoAplicacionCartaPorte.Eliminado;
            repositorio.GuardarCambios();
        }
        public List<ContratoParaAplicacionCartaPorte> ObtenerContratos(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);

            var codigoProveedor = usuario.ObtenerProveedorAsignado().CodigoProveedor;



            return consumer.ObtenerContratosProveedor("",codigoProveedor);

        }
        public List<CartaPorteParaAplicacionCartaPorte> ObtenerCartasPorte(string numeroContrato, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);

            var codigoProveedor = usuario.ObtenerProveedorAsignado().CodigoProveedor;

            return consumer.ObtenerCartasPorteProveedor(numeroContrato, codigoProveedor);
        }

        public void GuardarAplicacion(CrearAplicacionCartaPorte aplicacionACrear, string mailUsuario)
        {
            ValidarSchema(aplicacionACrear, "AplicacionCartaPorte", "GuardarAplicacion");
            if (!aplicacionACrear.ValidarKilogramos())
                throw new InfoCustomException("Revisar valor de KG.");
            var contratosValidos = ObtenerContratos(mailUsuario);
            if (!aplicacionACrear.ValidarContrato(contratosValidos))
                throw new InfoCustomException("Revisar contrato seleccionado.");
            var cartasPorteValidas = ObtenerCartasPorte(aplicacionACrear.Contrato, mailUsuario);
            if (!aplicacionACrear.ValidarCartaPorteSeleccionada(cartasPorteValidas))
                throw new InfoCustomException("Revisar carta porte seleccionada.");
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var aplicacion = new AplicacionCartaPorte(aplicacionACrear, usuario);
            repositorio.Agregar(aplicacion);

            repositorio.GuardarCambios();
        }

        private void ValidarSchema<T>(T schema, string controller, string metodo)
        {
            var validationContext = new ValidationContext(schema);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(schema, validationContext, validationResults))
            {
                Log.Debug(controller, metodo, string.Join("; ", validationResults.Select(valRes => valRes.ErrorMessage)));
                throw new InfoCustomException("Error validando formulario.");
            }
        }
    }
}
