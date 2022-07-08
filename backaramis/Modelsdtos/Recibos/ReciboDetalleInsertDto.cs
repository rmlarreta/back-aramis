namespace backaramis.Modelsdtos.Recibos
{
    public class ReciboDetalleInsertDto
    {
       public string Tipo { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Sucursal { get; set; } = null!;
        public decimal Monto { get; set; }
        public int Recibo { get; set; }
    }
}
