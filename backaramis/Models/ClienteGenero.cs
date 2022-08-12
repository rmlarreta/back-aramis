using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class ClienteGenero
    {
        public ClienteGenero()
        {
            Clientes = new HashSet<Cliente>();
        }

        public string Id { get; set; } = null!;

        public virtual ICollection<Cliente> Clientes { get; set; }
    }
}
