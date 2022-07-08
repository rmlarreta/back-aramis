namespace backaramis.Modelsdtos.Documents
{
    public class DocumentsDto
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Letra { get; set; }
        public int CodTipo { get; set; }
        public string? Tipo { get; set; }
        public int Pos { get; set; }
        public int Numero { get; set; }
        public long Cliente { get; set; }
        public long Cuit { get; set; }
        public string? Nombre { get; set; }
        public decimal Limite { get; set; }
        public string? Observaciones { get; set; } = null;
        public string? Operador { get; set; } = null;
        public decimal Total { get; set; }

    }
}
