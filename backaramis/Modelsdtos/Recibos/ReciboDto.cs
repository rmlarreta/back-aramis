using backaramis.Modelsdtos.Documents;

namespace backaramis.Modelsdtos.Recibos
{
    public class ReciboDto
    {
        public int Id { get; set; }
        public long Cliente { get; set; }
        public long Cuit { get; set; }
        public string? Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public string Operador { get; set; } = null!;
        public decimal Total { get; set; }
        public List<ReciboDetallDto>? Detalles { get; set; }
        public List<DocumentsDto>? Documents { get; set; }


    }
}
