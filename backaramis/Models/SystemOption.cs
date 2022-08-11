namespace backaramis.Models
{
    public partial class SystemOption
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int P { get; set; }
        public int R { get; set; }
        public int O { get; set; }
        public string Cuit { get; set; } = null!;
        public string Razon { get; set; } = null!;
        public string Domicilio { get; set; } = null!;
        public byte[]? Logo { get; set; }
        public string? Contacto { get; set; }
        public bool Produccion { get; set; }
        public long Lote { get; set; }
    }
}
