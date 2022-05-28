using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class ProductoRubro
    {
        public ProductoRubro()
        {
            Productos = new HashSet<Producto>();
        }

        public int Id { get; set; }
        public string Detalle { get; set; } = null!;

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
