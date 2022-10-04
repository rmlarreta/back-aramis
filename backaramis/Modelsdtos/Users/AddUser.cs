using System.ComponentModel.DataAnnotations;

namespace backaramis.Modelsdtos.Users
{
    public class AddUser
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string? Password { get; set; }
    }
}