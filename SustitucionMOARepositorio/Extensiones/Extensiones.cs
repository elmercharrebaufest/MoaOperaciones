using SustitucionMOAModel.Consultas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SustitucionMOARepositorio.Extensiones
{
    public static class Extensiones
    {
        public static decimal ToDecimal(this string str)
        {
            // you can throw an exception or return a default value here
            if (string.IsNullOrEmpty(str))
                return 0;

            decimal d;

            // you could throw an exception or return a default value on failure
            if (!decimal.TryParse(str, out d))
                return 0;

            return d;
        }
        public static ListaPaginada<TEntidad> OrdenarPaginarLista<TEntidad>(this IQueryable<TEntidad> resultado, Paginacion paginacion)
        {
            int itemsTotales = resultado.Count();

            resultado = ListarProyeccionQueryable(resultado, paginacion.OrdenarPor, paginacion.DireccionOrden, 0);

            resultado = resultado.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TEntidad>(resultado.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
        private static IQueryable<TProyeccion> ListarProyeccionQueryable<TProyeccion>(IQueryable<TProyeccion> resultadoFinal, string orden, DirOrden direccionOrden, int maxResultados)
        {

            if (orden != null)
            {
                var selectorOrden = Expresiones.Propiedad<TProyeccion>(orden);
                resultadoFinal = direccionOrden == DirOrden.Asc
                                 ? resultadoFinal.OrderBy(selectorOrden)
                                 : resultadoFinal.OrderByDescending(selectorOrden);
            }
            //CAMBIE ESTO ARA ACA ABAJO
            if (maxResultados != 0)
            {
                resultadoFinal = resultadoFinal.Take(maxResultados);
            }

            return resultadoFinal;
        }

        public static string[] SplitParagraph(this string paragraph, int maxCharacters)
        {
            List<string> linesList = new List<string>();
            StringBuilder currentLine = new StringBuilder();

            // Normalizamos saltos de línea
            string normalized = paragraph.Replace("\r", "");

            // Recorremos el texto por palabras, pero manteniendo los saltos de línea
            int start = 0;
            while (start < normalized.Length)
            {
                // Buscar el siguiente espacio o salto de línea
                int spaceIndex = normalized.IndexOf(' ', start);
                int newlineIndex = normalized.IndexOf('\n', start);
                int nextBreak;

                bool isNewline = false;

                if (spaceIndex == -1 && newlineIndex == -1)
                {
                    nextBreak = normalized.Length;
                }
                else if (spaceIndex == -1)
                {
                    nextBreak = newlineIndex;
                    isNewline = true;
                }
                else if (newlineIndex == -1)
                {
                    nextBreak = spaceIndex;
                }
                else
                {
                    nextBreak = Math.Min(spaceIndex, newlineIndex);
                    isNewline = nextBreak == newlineIndex;
                }

                string word = normalized.Substring(start, nextBreak - start);

                if (currentLine.Length == 0)
                {
                    currentLine.Append(word);
                }
                else
                {
                    if (currentLine.Length + 1 + word.Length > maxCharacters)
                    {
                        linesList.Add(currentLine.ToString());
                        currentLine.Clear();
                        currentLine.Append(word);
                    }
                    else
                    {
                        currentLine.Append(" ").Append(word);
                    }
                }

                // Si encontramos un salto de línea en el texto original, cortamos línea
                if (isNewline)
                {
                    linesList.Add(currentLine.ToString());
                    currentLine.Clear();
                }

                // Avanzamos al siguiente segmento
                start = nextBreak + 1;
            }

            // Agregar la última línea
            if (currentLine.Length > 0)
                linesList.Add(currentLine.ToString());

            return linesList.ToArray();
        }




        public static void SqlBulkInsert(this DbContext session, DataTable dataTable, string tableName)
        {
            var conn = ConfigurationManager.ConnectionStrings["CONTEXTO"].ConnectionString;//session.Database.Connection.ConnectionString;
            using (var copy = new SqlBulkCopy(conn))
            {
                copy.BulkCopyTimeout = 10000;
                copy.DestinationTableName = tableName;
                foreach (DataColumn column in dataTable.Columns)
                {
                    copy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                copy.WriteToServer(dataTable);
            }
        }

    }
}
