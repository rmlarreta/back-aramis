using AfipServiceReference;

namespace backaramis.Modelsdtos.Documents
{
    public class DocumentoFiscal
    {
        public int TipoComprobante { get; set; }
        public int TipoDocumento { get; set; }
        public long DocumentoCliente { get; set; }
        public decimal TotalComprobante { get; set; }
        public decimal NoGravado { get; set; }
        public decimal Neto { get; set; }
        public decimal Exento { get; set; }
        public decimal IvaTotal { get; set; }
        public decimal Neto10 { get; set; }
        public decimal Iva10 { get; set; }
        public decimal Neto21 { get; set; }
        public decimal Iva21 { get; set; }
        public decimal Internos { get; set; }
        public List<AlicIva>? Alicuotas { get; set; }
        public List<Tributo>? Tributo { get; set; }
    }

}
