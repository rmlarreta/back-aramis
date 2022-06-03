namespace backaramis.Models
{
    public partial class ProductoDetalle
    {
        public long Id { get; set; }
        public long ProductoFk { get; set; }
        public long ProductoDetalleFk { get; set; }
        public decimal Cantidad { get; set; }

        public virtual Producto ProductoDetalleFkNavigation { get; set; } = null!;
        public virtual Producto ProductoFkNavigation { get; set; } = null!;
    }
}
