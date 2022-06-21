using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class DocumentoEstado
    {
        public DocumentoEstado()
        {
            Documentos = new HashSet<Documento>();
        }

        public int Id { get; set; }
        public string Detalle { get; set; } = null!;

        public virtual ICollection<Documento> Documentos { get; set; }
    }
}
