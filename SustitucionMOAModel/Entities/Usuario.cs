using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SustitucionMOAModel.Entities
{
    public class Usuario
    {
        private List<PermisoEnum> permisosDelUsuario = null;

        [Key]
        public int Id { get; set; }
        public virtual string Mail { get; set; }
        public string CUITRegistro { get; set; }
        public bool Habilitado { get; set; }
        public string SeccionesVisitadas { get; set; }

        public virtual TipoUsuario TipoUsuario { get; set; }

        public DateTime? UltimoLogin { get; set; }

        [InverseProperty("UsuariosAsociados")]
        public virtual ICollection<Proveedor> Proveedores { get; set; }
        [InverseProperty("Usuarios")]
        public virtual ICollection<Rol> Roles { get; set; }
        public bool AceptoTyC { get; set; }
        public DateTime? AceptoTyCFecha { get; set; }
        public string ApiKey { get; set; }
        //public virtual ICollection<Archivo> Archivos { get; set; }
        public string UsuarioSap { get; set; }

        [InverseProperty("Usuario")]
        public virtual ICollection<PeticionDeOferta> Peticiones { get; set; }

        public Rol ObtenerRolPrincipal()
        {
            return Roles.FirstOrDefault();
        }

        public Proveedor ObtenerProveedor()
        {
            //Por ahora los usuarios van a tener solo un proveedor. Devolvemos ese
            // ya no son mas uno solo. :(
            if (Proveedores == null)
                return null;

            Proveedor proveedor = null;
            try
            {
                proveedor = Proveedores.Where(p => p.CUIT == this.CUITRegistro && this.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault();
            }
            catch (Exception)
            {
                proveedor = Proveedores.Where(p => p.CUIT == this.CUITRegistro).FirstOrDefault();
            }

            if (proveedor == null)
            {
                proveedor = Proveedores.FirstOrDefault();
            }

            return proveedor;
        }

        public Proveedor ObtenerCorredor()
        {
            return Proveedores.Where(p => p.CUIT == this.CUITRegistro && p.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor).FirstOrDefault();
        }

        public Proveedor ObtenerProveedorPorId(int proveedorId)
        {
            if (proveedorId > 0)
                return Proveedores.Where(p => p.Id == proveedorId).FirstOrDefault();
            else
                return Proveedores.FirstOrDefault();
        }
        public Proveedor ObtenerProveedorPorCodigo(string codigoProveedor)
        {
            return Proveedores.Where(p => p.CodigoProveedor == codigoProveedor).FirstOrDefault();
        }

        public Proveedor ObtenerProveedorPorCUIT(string CUIT)
        {
            return Proveedores.Where(p => p.CUIT == CUIT).FirstOrDefault();
        }

        public bool TieneProveedor(string codigoProveedor)
        {
            //Los administradores pueden elegir impersonarse como cualquier proveedor
            if (Roles.Where(r => r.Codigo == "ADM").Any())
            {
                return true;
            }

            return Proveedores.Where(p => p.CodigoProveedor == codigoProveedor).Any();
        }

        public string ObtenerRazonSocial()
        {
            if (Proveedores.Count >= 1)
            {
                if (!string.IsNullOrEmpty(ObtenerProveedor().RazonSocial))
                    return ObtenerProveedor().RazonSocial;
                else
                    return "No definido";
            }
            else
            {
                return "";
            }
        }

        public string ObtenerCodigoProveedor()
        {
            if (Proveedores.Count >= 1)
            {
                if (!string.IsNullOrEmpty(ObtenerProveedor().CodigoProveedor))
                    return ObtenerProveedor().CodigoProveedor;
                else
                    return "-";
            }
            else
            {
                return "";
            }
        }

        public List<string> ObtenerPermisos()
        {
            List<string> permisosUsuario = new List<string>();
            foreach (Rol Rol in Roles)
            {
                permisosUsuario.AddRange(Rol.ObtenerPermisos());
            }

            return permisosUsuario;
        }

        public bool EstaHabilitado()
        {
            return Habilitado;
        }

        public bool EsNuevoUsuario()
        {
            return
                Roles.Where(r => r.Codigo.Equals("NUEG")).Any() ||
                Roles.Where(r => r.Codigo.Equals("DDAG")).Any() ||
                Roles.Where(r => r.Codigo.Equals("NOIMP")).Any() ||
                Roles.Where(r => r.Codigo.Equals("NUECORR")).Any() ||
                Roles.Where(r => r.Codigo.Equals("NUENOGRAN")).Any() ||
                Roles.Where(r => r.Codigo.Equals("NUECLI")).Any() ||
                !Habilitado;
        }

        public bool EsCorredor()
        {
            return TipoUsuario.NombreCorto == "CORR";
        }

        public void RemoverRoles()
        {
            Roles.Clear();
        }

        public void RemoverRolesEditables()
        {
            Roles = Roles.Where(r => !r.EsEditable).ToList();
        }

        public void AgregarRol(Rol rol)
        {
            Roles.Add(rol);
        }

        public void RemoverRol(string rol)
        {
            var rolRemover = Roles.Where(r => r.Codigo == rol).FirstOrDefault();

            if (rolRemover != null)
                Roles.Remove(rolRemover);
        }


        public bool EsAdmin()
        {
            return Roles.Where(r => r.Codigo == "ADM").Any()
                    || Roles.Where(r => r.Codigo == "TODOS").Any();
        }


        [Obsolete("Reemplazar por método TienePermiso(Permiso permiso)", false)]
        public virtual bool TienePermiso(string permiso)
        {
            var permisosUsuario = ObtenerPermisos();

            return permisosUsuario.Contains(permiso);
        }

        public bool TienePermiso(PermisoEnum permiso)
        {
            if (permisosDelUsuario == null)
            {
                CargarPermisosUsuario();
            }
            return permisosDelUsuario.Contains(permiso);
        }

        public virtual bool TieneRol(string codigo)
        {

            return Roles.Any(r => r.Codigo == codigo);
        }

        private void CargarPermisosUsuario()
        {
            permisosDelUsuario = new List<PermisoEnum>();
            foreach (var rol in Roles)
            {
                rol.ObtenerPermisos().ForEach(r => permisosDelUsuario.Add(ObtenerPermisoEnum(r)));
            }
        }

        private static PermisoEnum ObtenerPermisoEnum(string permisoStr)
        {
            switch (permisoStr)
            {
                case "ABM BALANZAS": return PermisoEnum.AbmBalanzas;
                case "ABM COMMODITIES": return PermisoEnum.AbmCommodities;
                case "ABM EXPORTADORES": return PermisoEnum.AbmExportadores;
                case "ABM USUARIOS": return PermisoEnum.AbmUsuarios;
                case "CAMBIAR CONTRASENIA": return PermisoEnum.CambiarContrasenia;
                case "CARGAR FACT PROV": return PermisoEnum.CargarFactProv;
                case "CONSULTAR CAMARAS CONSOLIDACIO": return PermisoEnum.ConsultarCamarasConsolidacio;
                case "CONSULTAR CAMARAS MUELLE": return PermisoEnum.ConsultarCamarasMuelle;
                case "CONSULTAR CARTAS PORTE": return PermisoEnum.ConsultarCartasPorte;
                case "CONSULTAR CARTAS PORTE DETALLE": return PermisoEnum.ConsultarCartasPorteDetalle;
                case "CONSULTAR COMPROBANTES": return PermisoEnum.ConsultarComprobantes;
                case "CONSULTAR CONTRATO DETALLE": return PermisoEnum.ConsultarContratoDetalle;
                case "CONSULTAR CONTRATOS": return PermisoEnum.ConsultarContratos;
                case "CONSULTAR CUENTA CORRIENTE": return PermisoEnum.ConsultarCuentaCorriente;
                case "CONSULTAR DATOS FISCALES": return PermisoEnum.ConsultarDatosFiscales;
                case "CONSULTAR DOCUMENTACION": return PermisoEnum.ConsultarDocumentacion;
                case "CONSULTAR FLETE": return PermisoEnum.ConsultarFlete;
                case "CONSULTAR HOME": return PermisoEnum.ConsultarHome;
                case "CONSULTAR HOME NG": return PermisoEnum.ConsultarHomeNg;
                case "CONSULTAR INFORMACION METEOROL": return PermisoEnum.ConsultarInformacionMeteorol;
                case "CONSULTAR INFORME": return PermisoEnum.ConsultarInforme;
                case "CONSULTAR LIQUIDACIONES": return PermisoEnum.ConsultarLiquidaciones;
                case "CONSULTAR LIQUIDACIONES NG": return PermisoEnum.ConsultarLiquidacionesNg;
                case "CONSULTAR LISTADO PESADAS": return PermisoEnum.ConsultarListadoPesadas;
                case "CONSULTAR PAGOS": return PermisoEnum.ConsultarPagos;
                case "CONSULTAR PAGOS DETALLE": return PermisoEnum.ConsultarPagosDetalle;
                case "CONSULTAR PAGOS NG": return PermisoEnum.ConsultarPagosNg;
                case "CONSULTAR PESADA DETALLE": return PermisoEnum.ConsultarPesadaDetalle;
                case "CONSULTAR PESADAS": return PermisoEnum.ConsultarPesadas;
                case "CONSULTAR VENDEDOR STATUS": return PermisoEnum.ConsultarVendedorStatus;
                case "CONSULTAR VENDEDORES": return PermisoEnum.ConsultarVendedores;
                case "CONTACTO MAIL": return PermisoEnum.ContactoMail;
                case "CREAR FORMULARIO CCPP": return PermisoEnum.CrearFormularioCcpp;
                case "DATAAGROLOGIN": return PermisoEnum.Dataagrologin;
                case "DESCARGAR CARTAS PORTE": return PermisoEnum.DescargarCartasPorte;
                case "DESCARGAR CARTAS PORTE DETALLE": return PermisoEnum.DescargarCartasPorteDetalle;
                case "DESCARGAR COMPROBANTES": return PermisoEnum.DescargarComprobantes;
                case "DESCARGAR CONTRATO DETALLE": return PermisoEnum.DescargarContratoDetalle;
                case "DESCARGAR CONTRATOS": return PermisoEnum.DescargarContratos;
                case "DESCARGAR CUENTA CORRIENTE": return PermisoEnum.DescargarCuentaCorriente;
                case "DESCARGAR DOCUMENTO": return PermisoEnum.DescargarDocumento;
                case "DESCARGAR LIQUIDACIONES": return PermisoEnum.DescargarLiquidaciones;
                case "DESCARGAR LIQUIDACIONES NG": return PermisoEnum.DescargarLiquidacionesNg;
                case "DESCARGAR PAGOS": return PermisoEnum.DescargarPagos;
                case "DESCARGAR PAGOS DETALLE": return PermisoEnum.DescargarPagosDetalle;
                case "DESCARGAR PAGOS NG": return PermisoEnum.DescargarPagosNg;
                case "PESIFICACION": return PermisoEnum.Pesificacion;
                case "REGISTRAR PESADA": return PermisoEnum.RegistrarPesada;
                case "SELECCIONAR VENDEDOR": return PermisoEnum.SeleccionarVendedor;
                case "ALTA EMPRESA GRANOS": return PermisoEnum.AltaEmpresaGranos;
                case "ABM EMPRESAS": return PermisoEnum.AbmEmpresas;
                case "ABM EMPRESAS OPERADOR": return PermisoEnum.AbmEmpresasOperador;
                case "ABM EMPRESAS APROBADOR": return PermisoEnum.AbmEmpresasAprobador;
                case "ESTADO SOLICITUD": return PermisoEnum.EstadoSolicitud;
                case "CONSULTAR VENDEDOR PENDIENTES": return PermisoEnum.ConsultarVendedorPendientes;
                case "MENU": return PermisoEnum.Menu;
                case "NUEVO VENDEDOR": return PermisoEnum.NuevoVendedor;
                case "ABM NOTIFICACIONES": return PermisoEnum.AbmNotificaciones;
                case "ALTA EMPRESA NO GRANOS": return PermisoEnum.AltaEmpresaNoGranos;
                case "VER ALTAS GRANOS": return PermisoEnum.VerAltasGranos;
                case "VER ALTAS NO GRANOS": return PermisoEnum.VerAltasNoGranos;
                case "VER PESIFICACIONES": return PermisoEnum.VerPesificaciones;
                case "ELEGIR TODOS VENDEDORES": return PermisoEnum.ElegirTodosVendedores;
                case "CARGAR CONSULTA": return PermisoEnum.CargarConsulta;
                case "INFORMAR LIQUIDACION": return PermisoEnum.InformarLiquidacion;
                case "GUARDADO Y CONSULTA DE LOG PESIFICACIONES": return PermisoEnum.GuardadoYConsultaDeLogPesificaciones;
                case "CREAR CONTRATOS": return PermisoEnum.CrearContratos;
                case "ABM CAMPOS SUSTENTABLE": return PermisoEnum.AbmCamposSustentable;
                case "VER TODOS CAMPOS SUSTENTABLE": return PermisoEnum.VerTodosCamposSustentable;
                case "EDICION CAMPOS CREADOS": return PermisoEnum.EdicionCamposCreados;
                case "APIKEY": return PermisoEnum.Apikey;
                case "CONSULTA ABM": return PermisoEnum.ConsultaAbm;
                case "ALTA INTERNA GRANOS": return PermisoEnum.AltaInternaGranos;
                case "GESTION IMPUESTOS CM05": return PermisoEnum.GestionImpuestosCm05;
                case "BORRAR CAMPOS CREADOS": return PermisoEnum.BorrarCamposCreados;
                case "ABM SOLP": return PermisoEnum.AbmSolp;
                case "VER TODAS SOLPS": return PermisoEnum.VerTodasSolps;
                case "ACCESO QR": return PermisoEnum.AccesoQr;
                case "VER ORDENES DE CARGA DE TERCEROS": return PermisoEnum.VerOrdenesDeCargaDeTerceros;
                case "VER TODAS ORDENES DE CARGA": return PermisoEnum.VerTodasOrdenesDeCarga;
                case "VER ORDENES DE CARGA PARA COMERCIALES": return PermisoEnum.VerOrdenesDeCargaParaComerciales;
                case "VER ORDENES DE CARGA PARA MESA FAS": return PermisoEnum.VerOrdenesDeCargaParaMesaFas;
                case "VER ORDENES DE CARGA PARA PUERTO": return PermisoEnum.VerOrdenesDeCargaParaPuerto;
                case "MOSTRAR BUSCADOR INTELIGENTE": return PermisoEnum.MostrarBuscadorInteligente;
                case "CESIÓN Y RECTIFICACIÓN DE CPE": return PermisoEnum.CesiónYRectificaciónDeCpe;
                case "ALTA INTERNA NO GRANOS": return PermisoEnum.AltaInternaNoGranos;
                case "ANULAR ORDEN DE CARGA": return PermisoEnum.AnularOrdenDeCarga;
                case "VER ECHEQ": return PermisoEnum.VerEcheq;
                case "VER ECHEQ ADMIN": return PermisoEnum.VerEcheqAdmin;
                case "VER ORDENES DE CARGA FASON": return PermisoEnum.VerOrdenesDeCargaFason;
                case "VER ORDENES DE CARGA FASON ADMIN": return PermisoEnum.VerOrdenesDeCargaFasonAdmin;
                case "ENVIAR A SAP": return PermisoEnum.EnviarASap;
                case "CORREDOR": return PermisoEnum.Corredor;
                case "ABM APLICACIONES CCPP": return PermisoEnum.AbmAplicacionesCcpp;
                case "ADMIN APLICACIONES CCPP": return PermisoEnum.AdminAplicacionesCcpp;
                case "VER SOLPS COMPRADOR": return PermisoEnum.VerSolpsComprador;
                case "VER SOLAPA COMPRA": return PermisoEnum.VerSolapaCompra;
                case "COMERCIAL CAMPOS SUSTENTABLES": return PermisoEnum.ComercialCamposSustentables;
                case "VER SOLPS PROVEEDOR": return PermisoEnum.VerSolpsProveedor;
                case "FAS - MODIFICAR CAMPO REVENTA": return PermisoEnum.Fas_ModificarCampoReventa;

                default: throw new Exception("Permiso no mapeado: " + permisoStr);
            }
        }
    }
}
