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

namespace SustitucionMOAUtils.Services
{
    public class AplicacionCartaPorteService : IAplicacionCartaPorteService
    {
        protected readonly IRepositorio repositorio;
        public AplicacionCartaPorteService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public Resultado Agregar(AplicacionCartaPorte aplicacionCCPP, string mailUsuario)
        {
            return new Resultado() { Mensaje = "Testeo exitoso" };
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
                throw new InfoCustomException("No se ha encontrado la aplicacion.");
            if (aplicacion.Estado != EstadoAplicacionCartaPorte.Pendiente)
                throw new InfoCustomException("No se puede eliminar la aplicacion.");
            aplicacion.Estado = EstadoAplicacionCartaPorte.Eliminado;
            repositorio.GuardarCambios();
        }
    }
}
