using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using SustitucionMOAAssets;
using SustitucionMOACustomException;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.DBMap.Pesada;
using SustitucionMOAModel.Models.DBMap.Pesada.Detalle;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOAModel.Models.DBMap.RYD.CargaPesada;
using SustitucionMOAUtils.Credentials;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.DBMethods
{
    public class DBService : IDBService
    {
        private string DatabaseConnectionString = DBCredential.getConnectionString();

        public PesadaWSResponse SqlSPReporte(int centro, string fechaInicio, string fechaFin)
        {
            try
            {
                DateTime fechaIncioDateTime, fechaFinDateTime;
                try
                {
                    fechaIncioDateTime = DateTime.Parse(fechaInicio);
                }
                catch
                {
                    try
                    {
                        fechaInicio = new string(fechaInicio.Where(c => c != '\u200E').ToArray());
                        fechaIncioDateTime = DateTime.Parse(fechaInicio);
                    }
                    catch (Exception e)
                    {
                        throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "incio"), e);
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

                PesadaWSResponse result = new PesadaWSResponse();
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Informe_BalanzaPuerto", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.Parameters.Add(new SqlParameter("FechaHoraDesde", fechaIncioDateTime));
                    cmd.Parameters.Add(new SqlParameter("FechaHoraHasta", fechaFinDateTime));
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pesada pesada = new Pesada() {
                                centro = reader.GetInt32(0),
                                id = reader.GetInt32(1),
                                fechaIncio = reader.GetDateTime(2).ToString("dd MMM yyyy HH:mm"),
                                balanza = reader.GetString(3),
                                totalEmbarcado = reader.GetDouble(4),
                                commodity = reader.GetString(5),
                                bodega = reader.GetString(6),
                                destino = reader.GetString(7),
                                exportador = reader.GetString(8),
                                vapor = reader.GetString(9),
                                pesoProgramado = SAPFormatter.FormatearCantidad(reader.GetInt32(10), "")
                            };
                            result.pesadas.Add(pesada);
                        }
                        reader.Close();
                    }

                    if (result.pesadas.Count == 0) {
                        throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pesadas"));
                    }

                    return result;

                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e) {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<PesadaDetalle> SqlSPDeltallePesada(int centro, int nroOrden)
        {
            try
            {
                List<PesadaDetalle> data = new List<PesadaDetalle>() { };
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Informe_BalanzaPuertoDet", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("Centro", centro));
                    cmd.Parameters.Add(new SqlParameter("NroOrden", nroOrden));
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PesadaDetalle pesadaDetalle = new PesadaDetalle();
                            pesadaDetalle.balanza = reader.GetString(3);
                            pesadaDetalle.fecha = reader.GetDateTime(4).ToString("dd MMM yyyy HH:mm");
                            pesadaDetalle.pesoBruto = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(5),"");
                            pesadaDetalle.pesoTara = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(6),"");
                            pesadaDetalle.pesoNeto = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(7),"");
                            pesadaDetalle.centro = reader.GetInt32(0);
                            pesadaDetalle.nroOrden = reader.GetInt32(1);
                            pesadaDetalle.linea = reader.GetInt32(2);
                            data.Add(pesadaDetalle);
                        }
                        reader.Close();
                    }

                    return data;

                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DbElement> SqlSPCBElement(string command, List<DbParameter> paramenters ) {
            try
            {
                List<DbElement> elements = new List<DbElement>() { };
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(command, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (DbParameter parameter in paramenters)
                    {
                        cmd.Parameters.Add(new SqlParameter(parameter.nombre, parameter.value));
                    }
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DbElement element = new DbElement();
                            try
                            {
                                element.value = reader.GetString(0).Trim();
                            }
                            catch
                            {
                                try
                                {
                                    element.value = reader.GetInt32(0).ToString().Trim();
                                }
                                catch
                                {
                                    element.value = reader.GetInt16(0).ToString().Trim();
                                }
                            }
                            element.label = reader.GetString(1).Trim();
                            elements.Add(element);
                        }
                        reader.Close();
                    }

                    return elements;
                }
            }
            catch (SqlException e) {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*public ArrayList SqlSPCBBalanzas(int centro, bool filtroSoloManuales)
        {
            ArrayList balanzas = new ArrayList();
            using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_cb_Balanzas", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Centro", centro));
                cmd.Parameters.Add(new SqlParameter("SoloManuales", filtroSoloManuales));
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DbElement balanza = new DbElement();
                        balanza.id = reader.GetString(0).Trim();
                        balanza.text = reader.GetString(1).Trim();
                        balanzas.Add(balanza);
                    }
                    reader.Close();
                }

                return balanzas;
            }
        }*/

        /*public ArrayList SqlSPCBBodegas()
        {
            ArrayList bodegas = new ArrayList();
            using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_cb_Bodegas", conn);
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DbElement bodega = new DbElement();
                        bodega.id = reader.GetInt32(0).ToString().Trim();
                        bodega.text = reader.GetString(1).Trim();
                        bodegas.Add(bodega);
                    }
                    reader.Close();
                }

                return bodegas;
            }
        }*/

        public List<DbElement> SqlSPCBCabezales(int centro)
        {
            try
            {
                List<DbElement> cabezales = new List<DbElement>() { };
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_cb_Cabezal", conn);
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DbElement cabezal = new DbElement();
                            cabezal.value = reader.GetString(0).Trim();
                            cabezal.label = reader.GetString(1).Trim();
                            cabezales.Add(cabezal);
                        }
                        reader.Close();
                    }

                    return cabezales;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*public ArrayList SqlSPCBCommodities()
        {
            ArrayList commodities = new ArrayList();
            using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_cb_Commodities", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DbElement commodity = new DbElement();
                        commodity.id = reader.GetInt32(0).ToString().Trim();
                        commodity.text = reader.GetString(1).Trim();
                        commodities.Add(commodity);
                    }
                    reader.Close();
                }

                return commodities;
            }
        }*/

        public Commodity SqlSPCBCommodity(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_cb_Commodity", conn);
                    cmd.Parameters.Add(new SqlParameter("ID", id));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    Commodity commodity = new Commodity();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            commodity.value = reader.GetInt32(0).ToString().Trim();
                            commodity.label = reader.GetString(1).Trim();
                            if (!reader.IsDBNull(2))
                            {
                                commodity.MaterialSap = reader.GetString(2).Trim();
                            }
                            if (!reader.IsDBNull(3))
                            {
                                commodity.AlmacenOrigen = reader.GetString(3).Trim();
                            }
                            reader.Close();
                            return commodity;
                        }
                        return commodity;
                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*public ArrayList SqlSPCBDestinos()
        {
            ArrayList destinos = new ArrayList();
            using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_cb_Destinos", conn);
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DbElement destino = new DbElement();
                        destino.id = reader.GetInt32(0).ToString().Trim();
                        destino.text = reader.GetString(1).Trim();
                        destinos.Add(destino);
                    }
                    reader.Close();
                }

                return destinos;
            }
        }*/

        /*public ArrayList SqlSPCBExportadores()
        {
            ArrayList exportadores = new ArrayList();
            using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_cb_Exportadores", conn);
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DbElement exportador = new DbElement();
                        exportador.id = reader.GetInt32(0).ToString().Trim();
                        exportador.text = reader.GetString(1).Trim();
                        exportadores.Add(exportador);
                    }
                    reader.Close();
                }

                return exportadores;
            }
        }*/

        public Exportador SqlSPCBExportador(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_cb_Exportador", conn);
                    cmd.Parameters.Add(new SqlParameter("ID", id));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    Exportador exportador = new Exportador();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exportador.value = reader.GetInt32(0).ToString().Trim();
                            exportador.label = reader.GetString(1).Trim();
                            if (!reader.IsDBNull(2))
                            {
                                exportador.AlmacenSap = reader.GetString(2).Trim();
                            }
                        }
                        reader.Close();
                    }

                    return exportador;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DbElement> SqlSPCBITCs(int centro)
        {
            try
            {
                List<DbElement> itcs = new List<DbElement>();
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_cb_ITC", conn);
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DbElement itc = new DbElement();
                            itc.value = reader.GetString(0).Trim();
                            itc.label = reader.GetString(1).Trim();
                            itcs.Add(itc);
                        }
                        reader.Close();
                    }

                    return itcs;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DbElement> SqlSPCBNroPuestos(int centro, int itcID)
        {
            try
            {
                List<DbElement> nroPuestos = new List<DbElement>();
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_cb_NroPuesto", conn);
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.Parameters.Add(new SqlParameter("ITC", itcID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DbElement nroPuesto = new DbElement();
                            nroPuesto.value = reader.GetInt16(0).ToString().Trim();
                            nroPuesto.label = reader.GetString(1).Trim();
                            nroPuestos.Add(nroPuesto);
                        }
                        reader.Close();
                    }

                    return nroPuestos;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DbElement> SqlSPCBTipoAccesos()
        {
            try
            {
                List<DbElement> tipoAccesos = new List<DbElement>();
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_cb_TipoAcceso", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DbElement tipoAcceso = new DbElement();
                            tipoAcceso.value = reader.GetString(0).Trim();
                            tipoAcceso.label = reader.GetString(1).Trim();
                            tipoAccesos.Add(tipoAcceso);
                        }
                        reader.Close();
                    }

                    return tipoAccesos;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*public ArrayList SqlSPCBTipoBalanzas()
        {
            ArrayList tipoBalanzas = new ArrayList();
            using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_cb_TipoBalanza", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DbElement tipoBalanza = new DbElement();
                        tipoBalanza.id = reader.GetString(0).Trim();
                        tipoBalanza.text = reader.GetString(1).Trim();
                        tipoBalanzas.Add(tipoBalanza);
                    }
                    reader.Close();
                }

                return tipoBalanzas;
            }
        }*/

        /*public ArrayList SqlSPCBVapores()
        {
            ArrayList vapores = new ArrayList();
            using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_cb_Vapores", conn);
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DbElement vapor = new DbElement();
                        vapor.id = reader.GetInt32(0).ToString().Trim();
                        vapor.text = reader.GetString(1).Trim();
                        vapores.Add(vapor);
                    }
                    reader.Close();
                }

                return vapores;
            }
        }*/

        public void SqlSPGrabarPesadaBalanzaPuerto(int centro, DateTime fechaInicio, string balanza, string commodity, string bodega, string destino, string exportador, string vapor, int pesoProgramado, int nroPesada, DateTime fechaPesada, double pesoTara, double pesoBruto, double pesoNeto, string error, string docSap)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Grabar_BalanzaPuerto", conn);
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.Parameters.Add(new SqlParameter("FECHAHORAINICIO", fechaInicio));
                    cmd.Parameters.Add(new SqlParameter("BALANZA", balanza));
                    cmd.Parameters.Add(new SqlParameter("COMMODITY", commodity));
                    cmd.Parameters.Add(new SqlParameter("BODEGA", bodega));
                    cmd.Parameters.Add(new SqlParameter("DESTINO", destino));
                    cmd.Parameters.Add(new SqlParameter("EXPORTADOR", exportador));
                    cmd.Parameters.Add(new SqlParameter("VAPOR", vapor));
                    cmd.Parameters.Add(new SqlParameter("PESOPROGRAMADO", pesoProgramado));
                    cmd.Parameters.Add(new SqlParameter("NROPESADA", nroPesada));
                    cmd.Parameters.Add(new SqlParameter("FECHAHORAPESADA", fechaPesada));
                    cmd.Parameters.Add(new SqlParameter("PESOTARA", pesoTara));
                    cmd.Parameters.Add(new SqlParameter("PESOBRUTO", pesoBruto));
                    cmd.Parameters.Add(new SqlParameter("PESONETO", pesoNeto));
                    cmd.Parameters.Add(new SqlParameter("ERROR", error != null ? error : ""));
                    cmd.Parameters.Add(new SqlParameter("DocSap", docSap != null ? docSap : Convert.DBNull));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    cmd.Connection.Close();
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SqlSPCerrarPesadaBalanzaPuerto(int centro, DateTime fechaInicio, string balanza)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_CerrarPesada_BalanzaPuerto", conn);
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.Parameters.Add(new SqlParameter("FECHAHORAINICIO", fechaInicio));
                    cmd.Parameters.Add(new SqlParameter("BALANZA", balanza));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    cmd.Connection.Close();
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public CargaPesadaBalanza SqlSPBalanzaEnProcesoData(int centro, string balanza)
        {
            try
            {
                CargaPesadaBalanza data = new CargaPesadaBalanza();
                data.pesadas = new List<PesadaBalanza>();
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_BalanzaPuerto_EnProceso", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("Centro", centro));
                    cmd.Parameters.Add(new SqlParameter("Balanza", balanza));
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (data.fecha == null)
                            {
                                data.fecha = reader.GetDateTime(0).ToString("dd/MM/yyyy HH:mm");
                                data.balanza = reader.GetString(1);
                                data.commodity = reader.IsDBNull(2) ? "" : reader.GetInt32(2).ToString();
                                data.bodega = reader.GetInt32(3).ToString();
                                data.destino = reader.GetInt32(4).ToString();
                                data.exportador = reader.IsDBNull(5) ? "" : reader.GetInt32(5).ToString();
                                data.vapor = reader.GetInt32(6).ToString();
                                data.pesoProgramado = reader.GetInt32(7);
                                data.pesoAcumulado = 0;
                            }
                            PesadaBalanza pesada = new PesadaBalanza();
                            pesada.numeroPesada = reader.GetInt32(8);
                            pesada.fecha = reader.GetDateTime(9).ToString("dd/MM/yyyy HH:mm");
                            pesada.pesoBruto = reader.GetDouble(10);
                            pesada.pesoTara = reader.GetDouble(11);
                            pesada.pesoNeto = reader.GetDouble(12);
                            data.pesoAcumulado += Convert.ToInt32(reader.GetDouble(12));
                            data.pesadas.Add(pesada);
                        }
                        reader.Close();
                    }
                    if (data.balanza == null) {
                        return null;
                    }
                    return data;

                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<Balanza> SqlSPBalanzaSearchABM(int centro, string codigo, string descripcion, string tipo, string cabezal, string codigoSap)
        {
            try
            {
                List<Balanza> balanzas = new List<Balanza>();
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Search_Balanzas", conn);
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.Parameters.Add(new SqlParameter("CODIGO", codigo != "" ? codigo : Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("DESCRIPCION", descripcion != "" ? descripcion : Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("TIPO", tipo != "" ? tipo : Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("CODCABEZAL", cabezal != "" ? cabezal : Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("CODIGOSAP", codigoSap != "" ? codigoSap : Convert.DBNull));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Balanza balanza = new Balanza();
                            balanza.centro = reader.GetInt32(0).ToString().Trim();
                            balanza.codigo = reader.GetString(1).Trim();
                            balanza.descripcion = reader.GetString(2).Trim();
                            balanza.centroEmisor = reader.GetString(3).Trim();
                            balanza.automatico = reader.GetBoolean(4);
                            balanza.toleria = reader.GetInt32(5).ToString().Trim();
                            balanza.tolerX = reader.GetDecimal(6).ToString().Trim();
                            balanza.tipo = reader.GetString(7).Trim();
                            balanza.pesoMaximo = reader.GetDecimal(8).ToString().Trim();
                            balanza.codigoSAP = reader.GetString(9).Trim();
                            balanza.codigoCabezal = reader.IsDBNull(10) ? null : reader.GetString(10).Trim();
                            balanza.nombrePc = reader.IsDBNull(11) ? null : reader.GetString(11).Trim();
                            balanza.itc = reader.IsDBNull(12) ? null : reader.GetString(12).Trim();
                            balanza.nroPuesto = reader.IsDBNull(13) ? null : reader.GetInt16(13).ToString().Trim();
                            balanza.tipoAcceso = reader.GetString(14).Trim();
                            balanza.tipoDesc = reader.GetString(15).Trim();
                            balanza.cabezalDesc = reader.IsDBNull(16) ? null : reader.GetString(16).Trim();
                            balanzas.Add(balanza);

                        }
                        reader.Close();
                    }

                    return balanzas;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string SqlSPBalanzaGuardarABM(int centro, string codigo, string descripcion, string centroemisor, bool automatico, int tolerancia, decimal tolerxmil, string tipo, decimal pesoMaximo, string codigoSap, string codCabezal, string itc, int nroPuesto, string tipoAcceso, int maxCereo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Insert_Balanzas", conn);
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.Parameters.Add(new SqlParameter("CODIGO", codigo));
                    cmd.Parameters.Add(new SqlParameter("DESCRIPCION", descripcion));
                    cmd.Parameters.Add(new SqlParameter("CENTROEMISOR", centroemisor));
                    cmd.Parameters.Add(new SqlParameter("AUTOMATICO", automatico));
                    cmd.Parameters.Add(new SqlParameter("TOLERANCIA", tolerancia));
                    cmd.Parameters.Add(new SqlParameter("TOLERXMIL", tolerxmil));
                    cmd.Parameters.Add(new SqlParameter("TIPO", tipo));
                    cmd.Parameters.Add(new SqlParameter("PESOMAXIMO", pesoMaximo));
                    cmd.Parameters.Add(new SqlParameter("CODIGOSAP", codigoSap));
                    cmd.Parameters.Add(new SqlParameter("CODCABEZAL", codCabezal));
                    cmd.Parameters.Add(new SqlParameter("ITC", itc));
                    cmd.Parameters.Add(new SqlParameter("NROPUESTO", nroPuesto));
                    cmd.Parameters.Add(new SqlParameter("TIPOACCESO", tipoAcceso));
                    cmd.Parameters.Add(new SqlParameter("MaxCereo", maxCereo != 0 ? maxCereo : Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("NroCertHabilitacion", Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("FechaVto", Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("CodigoLOT", Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("CodigoAduana", Convert.DBNull));
                    SqlParameter outputParam = new SqlParameter("MensajeError", SqlDbType.VarChar, 255);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    string mensajeError = cmd.Parameters["MensajeError"].Value.ToString();
                    cmd.Connection.Close();
                    return mensajeError;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

   //FALTA TERMINAR - FALTAN DATOS
        public string SqlSPBalanzaActualizarABM(int centro, string codigo, string descripcion, string centroemisor, bool automatico, int tolerancia, decimal tolerxmil, string tipo, decimal pesoMaximo, string codigoSap, string codCabezal, string itc, int nroPuesto, string tipoAcceso, int idUsuario, string Usuario, int maxCereo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Update_Balanzas", conn);
                    cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                    cmd.Parameters.Add(new SqlParameter("CODIGO", codigo));
                    cmd.Parameters.Add(new SqlParameter("DESCRIPCION", descripcion));
                    cmd.Parameters.Add(new SqlParameter("CENTROEMISOR", centroemisor));
                    cmd.Parameters.Add(new SqlParameter("AUTOMATICO", automatico));
                    cmd.Parameters.Add(new SqlParameter("TOLERANCIA", tolerancia));
                    cmd.Parameters.Add(new SqlParameter("TOLERXMIL", tolerxmil));
                    cmd.Parameters.Add(new SqlParameter("TIPO", tipo));
                    cmd.Parameters.Add(new SqlParameter("PESOMAXIMO", pesoMaximo));
                    cmd.Parameters.Add(new SqlParameter("CODIGOSAP", codigoSap));
                    cmd.Parameters.Add(new SqlParameter("CODCABEZAL", codCabezal));
                    cmd.Parameters.Add(new SqlParameter("ITC", itc));
                    cmd.Parameters.Add(new SqlParameter("NROPUESTO", nroPuesto));
                    cmd.Parameters.Add(new SqlParameter("TIPOACCESO", tipoAcceso));
                    cmd.Parameters.Add(new SqlParameter("MaxCereo", maxCereo != 0 ? maxCereo : Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("IdUsuario", Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("Usuario", Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("NroCertHabilitacion", Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("FechaVto", Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("CodigoLOT", Convert.DBNull));
                    cmd.Parameters.Add(new SqlParameter("CodigoAduana", Convert.DBNull));
                    SqlParameter outputParam = new SqlParameter("MensajeError", SqlDbType.VarChar, 255);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    string mensajeError = cmd.Parameters["MensajeError"].Value.ToString();
                    cmd.Connection.Close();
                    return mensajeError;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string SqlSPBalanzaBorrarABM(int centro, string codigo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("Sp_Delete_Balanzas", conn);
                        cmd.Parameters.Add(new SqlParameter("CENTRO", centro));
                        cmd.Parameters.Add(new SqlParameter("CODIGO", codigo));
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection.Open();
                        cmd.ExecuteReader();
                        cmd.Connection.Close();
                    }
                    catch
                    {
                        return "La balanza no se ha podido eliminar. Otros registros poseen referencias a dicho elemento.";
                    }
                    return "";
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SqlSPInsertCommodity(string descripcion, string materialSap, string almacenOrigen)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Insert_Commodity", conn);
                    cmd.Parameters.Add(new SqlParameter("DESCRIPCION", descripcion));
                    cmd.Parameters.Add(new SqlParameter("MATERIALSAP", materialSap));
                    cmd.Parameters.Add(new SqlParameter("ALMACENORIGEN", almacenOrigen));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    cmd.Connection.Close();
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SqlSPUpdateCommodity(int id, string descripcion, string materialSap, string almacenOrigen)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Update_Commodity", conn);
                    cmd.Parameters.Add(new SqlParameter("ID", id));
                    cmd.Parameters.Add(new SqlParameter("DESCRIPCION", descripcion));
                    cmd.Parameters.Add(new SqlParameter("MATERIALSAP", materialSap));
                    cmd.Parameters.Add(new SqlParameter("ALMACENORIGEN", almacenOrigen));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    cmd.Connection.Close();
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string SqlSPDeleteCommodity(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Delete_Commodity", conn);
                    cmd.Parameters.Add(new SqlParameter("ID", id));
                    SqlParameter outputParam = new SqlParameter("MSJ", SqlDbType.VarChar, 255);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    string mensajeError = cmd.Parameters["MSJ"].Value.ToString();
                    cmd.Connection.Close();
                    return mensajeError;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SqlSPInsertExportador(string descripcion, string almacenSap)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Insert_Exportador", conn);
                    cmd.Parameters.Add(new SqlParameter("DESCRIPCION", descripcion));
                    cmd.Parameters.Add(new SqlParameter("ALMACENSAP", almacenSap));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    cmd.Connection.Close();
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SqlSPUpdateExportador(int id, string descripcion, string almacenSap)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Update_Exportador", conn);
                    cmd.Parameters.Add(new SqlParameter("ID", id));
                    cmd.Parameters.Add(new SqlParameter("DESCRIPCION", descripcion));
                    cmd.Parameters.Add(new SqlParameter("ALMACENSAP", almacenSap));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    cmd.Connection.Close();
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string SqlSPDeleteExportador(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Delete_Exportador", conn);
                    cmd.Parameters.Add(new SqlParameter("ID", id));
                    SqlParameter outputParam = new SqlParameter("MSJ", SqlDbType.VarChar, 255);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                    string mensajeError = cmd.Parameters["MSJ"].Value.ToString();
                    cmd.Connection.Close();
                    return mensajeError;
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public PesadaBalanzaInforme SqlSPInformeBalanza(int centro, string balanza)
        {
            try
            {
                PesadaBalanzaInforme data = new PesadaBalanzaInforme();
                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_Informe_BalanzaPuertoAct", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("Centro", centro));
                    cmd.Parameters.Add(new SqlParameter("Balanza", balanza));
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (data.fecha == null)
                            {
                                data.fecha = reader.GetDateTime(0).ToString("dd MMM yyyy HH:mm");
                                data.balanza = reader.GetString(1);
                                data.commodity = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                data.bodega = reader.GetString(3);
                                data.destino = reader.GetString(4);
                                data.exportador = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                data.vapor = reader.GetString(6);
                                data.pesoProgramado = SAPFormatter.FormatearCantidadInt(reader.GetInt32(7),"");
                            }
                            PesadaBalanzaItemInforme pesada = new PesadaBalanzaItemInforme();
                            pesada.fecha = reader.GetDateTime(8).ToString("dd MMM yyyy HH:mm");
                            pesada.pesoBruto = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(9), "");
                            pesada.pesoTara = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(10), "");
                            pesada.pesoNeto = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(11), "");
                            data.pesadas.Add(pesada);
                        }
                        reader.Close();
                    }

                    return data;

                }
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ListadoEncabezadoPesadas SqlSPListadoPesadas(int empresa, string commodity, string exportador, string fechaInicio, string fechaFin)
        {
            try
            {
                ListadoEncabezadoPesadas data = new ListadoEncabezadoPesadas();
                DateTime fechaInicioDate, fechaFinDate;
                string fechaInicioDB, fechaFinDB; 
                double totalCarga = 0, totalT00ll06 = 0, totalT06ll12 = 0, totalT12ll18 = 0, totalT18ll24 = 0;
                data.fecha = DateTime.Now.ToString("dd MMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                data.empresa = "Puerto San Lorenzo";
                try {
                    fechaInicioDate = Convert.ToDateTime(fechaInicio);
                    fechaInicio = fechaInicioDate.ToString("dd MMM yyyy");
                    fechaInicioDB = fechaInicioDate.ToString("yyyyMMdd");
                } catch {
                    fechaInicio = "";
                    fechaInicioDB = "";

                }
                try {
                    fechaFinDate = Convert.ToDateTime(fechaFin + " 23:59:59");
                    fechaFin = fechaFinDate.ToString("dd MMM yyyy");
                    fechaFinDB = fechaFinDate.ToString("yyyyMMdd HH:mm:ss");
                } catch {
                    fechaFin = "";
                    fechaFinDB = "";
                }
                data.fechaInicioFin = fechaInicio + " - " + fechaFin;

                object commodityParam = (commodity != "") ? Convert.ToInt32(commodity) : Convert.DBNull;
                object exportadorParam = (exportador != "") ? Convert.ToInt32(exportador) : Convert.DBNull;

                string commodityEnc = "";
                string exportadorEnc = "";

                using (SqlConnection conn = new SqlConnection(DatabaseConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_List_Puerto", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("CENTRO", empresa));
                    cmd.Parameters.Add(new SqlParameter("Material", commodityParam));
                    cmd.Parameters.Add(new SqlParameter("Exportador", exportadorParam));
                    cmd.Parameters.Add(new SqlParameter("FechaHoraDesde", fechaInicioDB));
                    cmd.Parameters.Add(new SqlParameter("FechaHoraHasta", fechaFinDB));
                    cmd.Connection.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EncabezadoPesadas encPesada = new EncabezadoPesadas();
                            encPesada.numViaje = reader.GetInt32(2).ToString();
                            encPesada.vapor = reader.GetString(7);
                            encPesada.commodity = reader.GetString(8);
                            commodityEnc = encPesada.commodity;
                            encPesada.exportador = reader.GetString(9);
                            exportadorEnc = encPesada.exportador;
                            encPesada.fecha = Convert.ToDateTime(reader.GetString(6)).ToString("dd MMM yyyy");
                            encPesada.totalTNCargadas = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(12), "");
                            totalCarga += reader.GetDouble(12);
                            encPesada.t00ll06 = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(13), "");
                            totalT00ll06 += reader.GetDouble(13);
                            encPesada.t06ll12 = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(14), "");
                            totalT06ll12 += reader.GetDouble(14);
                            encPesada.t12ll18 = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(15), "");
                            totalT12ll18 += reader.GetDouble(15);
                            encPesada.t18ll24 = SAPFormatter.FormatearCantidadDouble(reader.GetDouble(16), "");
                            totalT18ll24 += reader.GetDouble(16);
                            data.pesadas.Add(encPesada);
                        }
                        reader.Close();

                        if (data.pesadas.Count != 0)
                        {
                            EncabezadoPesadas encPesadaTotal = new EncabezadoPesadas();
                            encPesadaTotal.fecha = "TOTAL";
                            encPesadaTotal.totalTNCargadas = SAPFormatter.FormatearCantidadDouble(totalCarga, "");
                            encPesadaTotal.t00ll06 = SAPFormatter.FormatearCantidadDouble(totalT00ll06, "");
                            encPesadaTotal.t06ll12 = SAPFormatter.FormatearCantidadDouble(totalT06ll12, "");
                            encPesadaTotal.t12ll18 = SAPFormatter.FormatearCantidadDouble(totalT12ll18, "");
                            encPesadaTotal.t18ll24 = SAPFormatter.FormatearCantidadDouble(totalT18ll24, "");
                            data.pesadas.Add(encPesadaTotal);
                        }
                    }

                    data.commodity = commodity != "" ? commodityEnc : "Todos";
                    data.exportador = exportador != "" ? exportadorEnc : "Todos";

                }

                return data;
            }
            catch (SqlException e)
            {
                if (e.Number == 53)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBComunicacion);
                }
                if (e.Number == 18456)
                {
                    throw new DBCustomException(ErrorMsg.ErrorDBLogin);
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
