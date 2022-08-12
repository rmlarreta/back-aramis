using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class ProductoIva
    {
        public ProductoIva()
        {
            Productos = new HashSet<Producto>();
        }

        public int Id { get; set; }
        public decimal Tasa { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
