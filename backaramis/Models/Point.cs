using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class Point
    {
        public string Id { get; set; } = null!;
        public string Public { get; set; } = null!;
        public string? Token { get; set; }
        public string Ubicacion { get; set; } = null!;
    }
}
