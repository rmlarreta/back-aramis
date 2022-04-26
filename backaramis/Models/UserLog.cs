using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class UserLog
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; } = null!;
        public string Modulo { get; set; } = null!;
        public string Operador { get; set; } = null!;
        public string Tipo { get; set; } = null!;
    }
}
