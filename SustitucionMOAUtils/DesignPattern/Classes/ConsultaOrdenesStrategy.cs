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
        public List<string> Names => new List<string>() {"Orden de Carga"};

        private readonly IRepositorio repositorio;
        private readonly IUsuarioService usuarioService;

        public ConsultaOrdenesStrategy(IRepositorio repositorio, IUsuarioService usuarioService)
        {
            this.repositorio = repositorio;
            this.usuarioService = usuarioService;
        }
        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario)
        {
            try
            {
                OrdenDeCarga orden = this.repositorio.Obtener<OrdenDeCarga>(o => o.Id == consulta.Detalle.Orden_Id);
                Usuario usuario = this.repositorio.Obtener<Usuario>(u => u.Id == orden.UsuarioCreacion_Id);
                Proveedor proveedor = usuario.Proveedores.First(p => p.Mail == usuario.Mail && p.CUIT == usuario.CUITRegistro);

                consulta.Usuario_Id = orden.UsuarioCreacion_Id;
                consulta.CodigoProveedor = proveedor.CodigoProveedor?? "-";
                consulta.RazonSocialProveedor = proveedor.RazonSocial;
                comentario.Usuario_Id = (int)consulta.UsuarioInterno_Id;

                this.repositorio.Agregar(consulta);
                this.repositorio.GuardarCambios();
                return consulta;
            }
            catch (Exception e)
            {
                Log.Info(e.Message);
                return null;
            }
        }
    }
}
