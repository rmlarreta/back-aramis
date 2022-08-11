namespace backaramis.Models
{
    public partial class DocumentoRecibo
    {
        public long Id { get; set; }
        public long Documento { get; set; }
        public int Recibo { get; set; }
        public decimal Monto { get; set; }

        public virtual Documento DocumentoNavigation { get; set; } = null!;
        public virtual Recibo ReciboNavigation { get; set; } = null!;
    }
}
