using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class DocumentoDetalle
    {
        public long Id { get; set; }
        public long Documento { get; set; }
        public decimal Cantidad { get; set; }
        public long Producto { get; set; }
        public string Plu { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public string Rubro { get; set; } = null!;
        public decimal Neto { get; set; }
        public decimal Iva { get; set; }
        public decimal Internos { get; set; }
    }
}
