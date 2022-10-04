using System.ComponentModel.DataAnnotations;

namespace backaramis.Modelsdtos.Users
{
    public class ChangePassword
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string? NPassword { get; set; }


    }
}
