using System.ComponentModel.DataAnnotations;

namespace backaramis.Modelsdtos.Users
{
    public class EditUser
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? Username { get; set; }

        public string? Password { get; set; }

        [Required]
        public int Perfil { get; set; }
        [Required]
        public DateTime EndOfLife { get; set; }
        [Required]
        public bool Confirmado { get; set; }
    }
}