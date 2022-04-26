using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class Cliente
    {
        public long Id { get; set; }
        public long Cuit { get; set; }
        public string Responsabilidad { get; set; } = null!;
        public string Genero { get; set; } = null!;
        public string Imputacion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? NombreFantasia { get; set; }
        public string Domicilio { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Mail { get; set; }
        public decimal LimiteSaldo { get; set; }
        public string? Observaciones { get; set; }
        public decimal Saldo { get; set; }

        public virtual ClienteGenero GeneroNavigation { get; set; } = null!;
        public virtual ProveedorImputacion ImputacionNavigation { get; set; } = null!;
        public virtual ClienteResponsabilidad ResponsabilidadNavigation { get; set; } = null!;
    }
}
