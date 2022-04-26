using System;
using System.Collections.Generic;

namespace backaramis.Models
{
    public partial class User
    {
        public long Id { get; set; }
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public DateTime EndOfLife { get; set; }
        public string Username { get; set; } = null!;
        public int Perfil { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool Confirmado { get; set; }

        public virtual UserPerfil PerfilNavigation { get; set; } = null!;
    }
}
