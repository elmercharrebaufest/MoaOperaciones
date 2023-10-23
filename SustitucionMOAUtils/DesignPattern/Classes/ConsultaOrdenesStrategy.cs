using Quartz.Util;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using SustitucionMOAUtils.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAUtils.Logger;
using System.IO;
using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.DesignPattern.Classes
{
    public class ConsultaOrdenesStrategy : IConsultaStrategy
    {
        private readonly IRepositorio repositorio;
        private readonly IUsuarioService usuarioService;
        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "RespuestaConsulta.html");

        public string Name => "Orden de Carga";
        public ConsultaOrdenesStrategy(IRepositorio repositorio, IUsuarioService usuarioService)
        {
            this.repositorio = repositorio;
            this.usuarioService = usuarioService;
        }
        public void AgregarConsulta(Consulta consulta, Comentario comentario)
        {
            try
            {
                OrdenDeCarga orden = this.repositorio.Obtener<OrdenDeCarga>(o => o.Id == consulta.Detalle.Orden_Id);
                Usuario usuario = this.repositorio.Obtener<Usuario>(u => u.Id == orden.UsuarioCreacion_Id);
                var usuarioDto = usuarioService.GetUsuario(usuario.Mail);
                consulta.Usuario_Id = usuarioDto.Id;
                var proveedor = usuario.Proveedores.First(p => p.Mail == usuario.Mail && p.CUIT == usuario.CUITRegistro);
                
                consulta.EstadoConsulta_Id = 4;
                comentario.Usuario_Id = consulta.UsuarioInterno_Id;
                consulta.CodigoProveedor = proveedor.CodigoProveedor;
                consulta.RazonSocialProveedor = proveedor.RazonSocial;

                this.repositorio.Agregar(consulta);
                
                this.repositorio.GuardarCambios();
            }catch(Exception e)
            {
                Log.Info(e.Message);
            }
        }
        public void EnviarMailInterno(Consulta consulta, Comentario comentario)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, consulta.Asunto, !string.IsNullOrWhiteSpace(comentario.Detalle) ? comentario.Detalle : "-");
                string asunto = "Molinos Agro - Consulta N°: " + consulta.Id + " - " + consulta.Asunto;

                EmailSender.EnviarMail(new List<string> { consulta.Usuario.Mail }, asunto, cuerpo, null, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
