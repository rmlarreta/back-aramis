using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class Point
    {
        public int Id { get; set; }
        public string? DeviceId { get; set; }
        public string? Token { get; set; }
        public string Ubicacion { get; set; } = null!;
    }
}
