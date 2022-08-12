using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class DocumentoTipo
    {
        public DocumentoTipo()
        {
            Documentos = new HashSet<Documento>();
        }

        public int Id { get; set; }
        public string Letra { get; set; } = null!;
        public string Detalle { get; set; } = null!;

        public virtual ICollection<Documento> Documentos { get; set; }
    }
}
