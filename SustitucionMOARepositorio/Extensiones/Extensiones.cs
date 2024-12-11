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
            int currentLineLength = 0;

            string[] words = paragraph.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a + '\n').SelectMany(word => word.Split(' ')).ToArray();

            foreach (string word in words)
            {
                // Si la palabra es más larga que el límite máximo, se divide en múltiples líneas
                if (word.Length > maxCharacters)
                {
                    int index = 0;
                    while (index < word.Length)
                    {
                        int length = Math.Min(maxCharacters, word.Length - index);
                        string subWord = word.Substring(index, length);

                        // Si la línea actual ya tiene contenido, agregarla a la lista y crear una nueva línea
                        if (currentLineLength > 0)
                        {
                            linesList.Add(currentLine.ToString().TrimEnd());
                            currentLine.Clear();
                            currentLineLength = 0;
                        }

                        // Agregar subpalabra a la línea actual y agregarla a la lista si es necesario
                        currentLine.Append(subWord).Append(" ");
                        linesList.Add(currentLine.ToString().TrimEnd());
                        currentLine.Clear();
                        currentLineLength = 0;

                        index += length;
                    }
                }
                else
                {
                    // Verificar si agregar la palabra excede el límite de caracteres o si es un punto seguido o un salto de línea
                    if (currentLineLength + word.Length + 1 > maxCharacters || word.EndsWith(".") || word.EndsWith("\n"))
                    {
                        // Agregar la palabra a la línea actual
                        currentLine.Append(word).Append(" ");
                        linesList.Add(currentLine.ToString().TrimEnd()); // Agregar la línea actual a la lista
                        currentLine.Clear(); // Crear una nueva línea vacía
                        currentLineLength = 0;
                    }
                    else
                    {
                        // Agregar la palabra a la línea actual
                        currentLine.Append(word).Append(" ");
                        currentLineLength += word.Length + 1;
                    }
                }
            }

            // Agregar la última línea si no está vacía
            if (currentLine.Length > 0)
                linesList.Add(currentLine.ToString().TrimEnd());

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
