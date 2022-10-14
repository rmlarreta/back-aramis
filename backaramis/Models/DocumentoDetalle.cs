namespace backaramis.Models
{
    public partial class DocumentoDetalle
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
        public decimal Facturado { get; set; }

        public virtual Documento DocumentoNavigation { get; set; } = null!;
    }
}
