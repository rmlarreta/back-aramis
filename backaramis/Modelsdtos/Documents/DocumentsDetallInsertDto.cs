namespace backaramis.Modelsdtos.Documents
{
    public class DocumentsDetallInsertDto
    {
        public long Id { get; set; }
        public long Documento { get; set; }
        public decimal Cantidad { get; set; }
        public long Producto { get; set; }
        public string Codigo { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public string Rubro { get; set; } = null!;
        public decimal Unitario { get; set; }
        public decimal Iva { get; set; }
        public decimal Internos { get; set; }
    }
}
