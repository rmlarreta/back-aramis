namespace backaramis.Modelsdtos.Recibos
{
    public class ReciboDetallDto
    {
        public long Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Sucursal { get; set; } = null!;
        public decimal Monto { get; set; }
    }
}
