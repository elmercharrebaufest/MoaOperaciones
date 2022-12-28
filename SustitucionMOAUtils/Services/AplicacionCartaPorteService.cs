using DocumentFormat.OpenXml.Office2010.Excel;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using Org.BouncyCastle.Asn1.Ocsp;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using System.Data.Entity;

namespace SustitucionMOAUtils.Services
{
    public class AplicacionCartaPorteService : IAplicacionCartaPorteService
    {
        protected readonly IRepositorio _repositorio;
        //protected readonly IAplicacionCartaPorteConsumerMOA _consumer;
        public AplicacionCartaPorteService(
            IRepositorio repositorio
            //IAplicacionCartaPorteConsumerMOA consumer,
            )
        {
            this._repositorio = repositorio;
            //this._consumer = consumer;
        }
        public Resultado Agregar(AplicacionCartaPorte aplicacionCCPP, string mailUsuario)
        {
            return new Resultado() { Mensaje = "Testeo exitoso" };
        }
        public List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin)
        {
            DateTime fechaInicioDateTime, fechaFinDateTime;
            try
            {
                fechaInicioDateTime = DateTime.Parse(fechaInicio);
            }
            catch
            {
                try
                {
                    fechaInicio = new string(fechaInicio.Where(c => c != '\u200E').ToArray());
                    fechaInicioDateTime = DateTime.Parse(fechaInicio);
                }
                catch (Exception e)
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "inicio"), e);
                }
            }

            try
            {
                fechaFinDateTime = DateTime.Parse(fechaFin);
            }
            catch
            {
                try
                {
                    fechaFin = new string(fechaFin.Where(c => c != '\u200E').ToArray());
                    fechaFinDateTime = DateTime.Parse(fechaFin);
                }
                catch (Exception e)
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "fin"), e);
                }
            }
            var usuario = _repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            string cuit = usuario.CUITRegistro;
            bool isAdmin = usuario.TienePermiso("ADMIN APLICACIONES CCPP");

            var rawList = _repositorio.Listar<AplicacionCartaPorte>(apl =>
                (isAdmin ? true : apl.Proveedor.CUIT == cuit) &&
                fechaInicioDateTime <= apl.FechaAlta && fechaFinDateTime >= DbFunctions.TruncateTime(apl.FechaAlta)
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
    }
}
