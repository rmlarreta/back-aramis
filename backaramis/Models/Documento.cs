using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class Documento
    {
        public long Id { get; set; }
        public int Tipo { get; set; }
        public int Pos { get; set; }
        public int Numero { get; set; }
        public long Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public string? Cai { get; set; }
        public DateTime? Vence { get; set; }
        public string Razon { get; set; } = null!;
        public string? Observaciones { get; set; }
        public string Operador { get; set; } = null!;
        public DateTime Creado { get; set; }
        public int Estado { get; set; }
        public int? Recibo { get; set; }
        public decimal? Pago { get; set; }

        public virtual DocumentoEstado EstadoNavigation { get; set; } = null!;
        public virtual DocumentoTipo TipoNavigation { get; set; } = null!;
    }
}
