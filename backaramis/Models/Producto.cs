namespace backaramis.Models
{
    public partial class Producto
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public int Rubro { get; set; }
        public decimal Costo { get; set; }
        public int Iva { get; set; }
        public decimal Internos { get; set; }
        public decimal Tasa { get; set; }
        public decimal Stock { get; set; }
        public bool Servicio { get; set; }

        public virtual ProductoIva IvaNavigation { get; set; } = null!;
        public virtual ProductoRubro RubroNavigation { get; set; } = null!;
    }
}
