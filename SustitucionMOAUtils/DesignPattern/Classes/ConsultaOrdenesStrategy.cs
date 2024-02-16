using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAUtils.Logger;
using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Helpers;
using System.Web;
using SustitucionMOAModel.CustomExceptions;

namespace SustitucionMOAUtils.DesignPattern.Classes
{
    public class ConsultaOrdenesStrategy : ConsultaCommon, IConsultaStrategy
    {
        public List<string> Names => new List<string>() {"Orden de Carga"};
        private readonly IUsuarioService usuarioService;

        public ConsultaOrdenesStrategy(IRepositorio repositorio, IEmailService emailService, IUsuarioService usuarioService) : base(repositorio, emailService)
        {
            this.usuarioService = usuarioService;
        }
        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario, List<DestinatarioDto> destinatarios, HttpFileCollectionBase files)
        {
            try
            {
                var consultaAUsuario = AgregarConsultaCreadorOrdenFAS(consulta, comentario);
                this.CompletarCamposYClonar(consultaAUsuario, comentario);
                this.repositorio.Agregar(consultaAUsuario);
                this.repositorio.GuardarCambios();

                if (files.Count > 0)
                {
                    GuardarAdjuntoComentario(consultaAUsuario.Id, files);
                }
                this.EnviarMail(consultaAUsuario, comentario, files, destinatarios);

                return consultaAUsuario;
            }
            catch (Exception e)
            {
                Log.Error("Ha ocurrido un error al intentar generar la consulta.", e);
                throw e;
            }
        }

        public void EnviarMail(Consulta consulta, Comentario comentario, HttpFileCollectionBase files, List<DestinatarioDto> destinatarios)
        {
            var destinatariosCC = this.emailService.ObtenerListaDestinatarios(new string[] { consultaInternaCC });
            var destinatariosMail = destinatarios.Select(d => d.Mail).ToList();
            var ccCliente = destinatarios.FirstOrDefault(u => u.Campo == "Cliente");
            if(ccCliente!= null)
            {
                destinatariosCC.Add(ccCliente.Mail);
            }
            this.EnviarMailInterno(consulta, comentario, files, destinatariosCC, destinatariosMail);       
        }

        private Consulta AgregarConsultaCreadorOrdenFAS(Consulta consulta, Comentario comentario)
        {
            Consulta consultaCreador = (Consulta)consulta.Clone();

            OrdenDeCarga orden = this.repositorio.Obtener<OrdenDeCarga>(o => o.Id == consulta.Detalle.Orden_Id);
            Usuario usuario = this.repositorio.Obtener<Usuario>(u => u.Id == orden.UsuarioCreacion_Id);
            Proveedor proveedor = usuario.Proveedores.FirstOrDefault(p => p.Mail == usuario.Mail && p.CUIT == usuario.CUITRegistro);

            if(proveedor == null)
            {
                throw new ValidationCustomException("Hubo un problema al intentar obtener datos del vendedor.");
            }

            consultaCreador.Usuario_Id = orden.UsuarioCreacion_Id;
            consultaCreador.EstadoConsulta_Id = 4;
            consultaCreador.CodigoProveedor = proveedor.CodigoProveedor ?? "-";
            consultaCreador.RazonSocialProveedor = proveedor.RazonSocial;
            comentario.Usuario_Id = (int)consultaCreador.UsuarioInterno_Id;
            return consultaCreador;
        }

        private void CompletarCamposYClonar(Consulta consulta, Comentario comentario)
        {
            consulta.FechaCreacion = DateTime.Now;
            consulta.FechaUltimaModificacion = DateTime.Now;

            Categoria categoria = repositorio.Obtener<Categoria>(c => c.Id == consulta.Categoria_Id);

            comentario.ComentarioRecordado = new List<ComentarioRecordado>();

            if (consulta.Comentarios == null)
            {
                consulta.Comentarios = new List<Comentario>();
            }

            consulta.Comentarios.Add(comentario.Clone() as Comentario);

            consulta.Detalle = consulta.Detalle.Clone() as ConsultaDetalle;

        }

    }
}
