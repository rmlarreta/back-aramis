using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class DocumentoDetalleRemitoFacturado
    {
        public long Id { get; set; }
        public long Detalle { get; set; }
        public long Factura { get; set; }
        public decimal Cantidad { get; set; }
    }
}
