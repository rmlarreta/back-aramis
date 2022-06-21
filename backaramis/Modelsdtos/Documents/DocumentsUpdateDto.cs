namespace backaramis.Modelsdtos.Documents
{
    public class DocumentsUpdateDto
    {
        public long Id { get; set; }
        public int Tipo { get; set; }
        public int Pos { get; set; }
        public int Numero { get; set; }
        public long Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public string? Cai { get; set; }
        public DateTime? Vence { get; set; }
        public string Razon { get; set; } = null!;
        public string? Observaciones { get; set; }
        public string Operador { get; set; } = null!;
        public DateTime Creado { get; set; }

    } 
}
