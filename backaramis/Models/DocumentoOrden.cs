using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class DocumentoOrden
    {
        public long Id { get; set; }
        public long Documento { get; set; }
        public long Orden { get; set; }
    }
}
