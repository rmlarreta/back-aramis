namespace backaramis.Modelsdtos.Clientes
{
    public class ClienteDto
    {
        public long Id { get; set; }
        public long Cuit { get; set; }
        public string Responsabilidad { get; set; } = null!;
        public string Genero { get; set; } = null!;
        public long Imputacion { get; set; } 
        public string Nombre { get; set; } = null!;
        public string? NombreFantasia { get; set; }
        public string Domicilio { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Mail { get; set; }
        public decimal LimiteSaldo { get; set; }
        public string? Observaciones { get; set; }
    }
}
