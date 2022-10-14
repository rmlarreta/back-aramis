namespace backaramis.Modelsdtos.Documents
{
    public class DocumentsDto
    {
        public long Id { get; set; }
        //HEADER
        //LOGO
        public int CodTipo { get; set; }
        public string? CodAut { get; set; }
        public string? Letra { get; set; }
        public string? Tipo { get; set; }
        public string? Pos { get; set; }
        public string? Numero { get; set; }
        public DateTime Fecha { get; set; }
        public int NumeroInt { get; set; }
        public int PosInt { get; set; }
        public DateTime Vence { get; set; }
        //EMPRESA
        public string? Razon { get; set; }
        public string? CuitEmpresa { get; set; }
        public string? Fantasia { get; set; }
        public string? IIBB { get; set; }
        public string? DomicilioEmpresa { get; set; }
        public string? InicioActividades { get; set; }
        public string? ResponsabilidadEmpresa { get; set; }

        //CLIENTE
        public string? Nombre { get; set; } //CLIENTE EN DOCUMENTO
        public long Cuit { get; set; }
        public long Cliente { get; set; }  // ID CLIENTE
        public string? DomicilioCliente { get; set; }
        public string? ResponsabilidadCliente { get; set; }

        //FOOTER
        public decimal Limite { get; set; }
        public string? Observaciones { get; set; } = null;
        public string? Operador { get; set; } = null;
        public decimal Total { get; set; }
        public decimal Neto { get; set; }
        public decimal Internos { get; set; }

        public decimal Neto10 { get; set; }
        public decimal Iva10 { get; set; }
        public decimal Neto21 { get; set; }
        public decimal Iva21 { get; set; }
        public decimal Excento { get; set; }
        public string? EstadoPago { get; set; } = null;
        public string? EnLetras { get; set; } = null;

    }
}
