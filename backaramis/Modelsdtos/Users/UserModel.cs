namespace backaramis.Modelsdtos.Users
{
    public class UserModel
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public DateTime EndOfLife { get; set; }
        public int Perfil { get; set; }
        public bool Confirmado { get; set; }
    }
}