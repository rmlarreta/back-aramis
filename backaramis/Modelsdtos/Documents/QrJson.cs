namespace backaramis.Modelsdtos.Documents
{
    public class QrJson
    {
        public int Ver { get; set; }
        public string? Fecha { get; set; } //	"2020-10-13"
        public string? Cuit { get; set; }
        public int PtoVenta { get; set; }
        public int TipoCmp { get; set; }
        public int NroCmp { get; set; }
        public decimal Importe { get; set; }
        public string? Moneda { get; set; }
        public decimal Ctz { get; set; }
        public int TipoDocRec { get; set; }
        public long NroDocRec { get; set; }
        public string? TipoCodAut { get; set; }
        public string? CodAut { get; set; }

    }
}
