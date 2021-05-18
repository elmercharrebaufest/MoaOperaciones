using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class ConflictoCampoSustentable
    {
        [Key, Column(Order = 0)]
        public int IdCampo { get; set; }

        [Key, Column(Order = 1)]
        public int IdTSA { get; set; }

        [Key, Column(Order = 2)]
        public string CUIT { get; set; }

        [Key, Column(Order = 3)]
        public double ToneladasInformadas { get; set; }

        public int IdCampoSustentable { get; set; }

        public double ToneladasActuales { get; set; }

        public double StockDisponible { get; set; }

        public string MotivoRechazo { get; set; }

        public DateTime FechaConflicto { get; set; }

        public bool Notificado { get; set; }

    }
}
