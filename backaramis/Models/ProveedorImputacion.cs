namespace backaramis.Models
{
    public partial class ProveedorImputacion
    {
        public ProveedorImputacion()
        {
            Clientes = new HashSet<Cliente>();
        }

        public long Id { get; set; }
        public string Detalle { get; set; } = null!;

        public virtual ICollection<Cliente> Clientes { get; set; }
    }
}
