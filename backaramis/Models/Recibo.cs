using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class Recibo
    {
        public Recibo()
        {
            DocumentoRecibos = new HashSet<DocumentoRecibo>();
            ReciboDetalles = new HashSet<ReciboDetalle>();
        }

        public int Id { get; set; }
        public long Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public string Operador { get; set; } = null!;

        public virtual Cliente ClienteNavigation { get; set; } = null!;
        public virtual ICollection<DocumentoRecibo> DocumentoRecibos { get; set; }
        public virtual ICollection<ReciboDetalle> ReciboDetalles { get; set; }
    }
}
