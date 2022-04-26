using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class UserOperation
    {
        public UserOperation()
        {
            UserPerfilOperations = new HashSet<UserPerfilOperation>();
        }

        public int Id { get; set; }
        public string Operacion { get; set; } = null!;
        public int Modulo { get; set; }

        public virtual UserModule ModuloNavigation { get; set; } = null!;
        public virtual ICollection<UserPerfilOperation> UserPerfilOperations { get; set; }
    }
}
