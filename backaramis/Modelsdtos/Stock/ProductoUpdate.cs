namespace backaramis.Modelsdtos.Stock
{
    public class ProductoUpdate
    {
        public long Id { get; set; }
        public string Codigo { get; set; }  
        public string Detalle { get; set; } 
        public int Rubro { get; set; } 
        public decimal Costo { get; set; }
        public int Iva { get; set; } 
        public decimal Internos { get; set; }
        public decimal Tasa { get; set; } 
        public bool Servicio { get; set; }
        public decimal Stock { get; set; }
    }
}
