using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class ReciboDetalle
    {
        public long Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Sucursal { get; set; } = null!;
        public decimal Monto { get; set; }
        public int Recibo { get; set; }

        public virtual Recibo ReciboNavigation { get; set; } = null!;
    }
}
