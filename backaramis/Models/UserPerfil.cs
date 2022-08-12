using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class UserPerfil
    {
        public UserPerfil()
        {
            UserPerfilOperations = new HashSet<UserPerfilOperation>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Rol { get; set; } = null!;

        public virtual ICollection<UserPerfilOperation> UserPerfilOperations { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
