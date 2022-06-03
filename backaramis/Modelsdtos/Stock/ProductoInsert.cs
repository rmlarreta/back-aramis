namespace backaramis.Modelsdtos.Stock
{
    public class ProductoInsert
    {
        public string Codigo { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public int Rubro { get; set; }
        public decimal Costo { get; set; }
        public int Iva { get; set; }
        public decimal Internos { get; set; }
        public decimal Tasa { get; set; }
        public bool Servicio { get; set; }
    }
}
