using backaramis.Modelsdtos.Documents;

namespace backaramis.Modelsdtos.Recibos
{
    public class ReciboInsertDto
    { 
        public long Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public string Operador { get; set; } = null!;
        public IEnumerable<ReciboDetalleInsertDto>? ReciboDetalles { get; set; }
        public IEnumerable<DocumentsDto>? Documents { get; set; }
    }
}
