namespace backaramis.Models
{
    public partial class UserPerfilOperation
    {
        public int Id { get; set; }
        public int IdPerfil { get; set; }
        public int IdOperacion { get; set; }

        public virtual UserOperation IdOperacionNavigation { get; set; } = null!;
        public virtual UserPerfil IdPerfilNavigation { get; set; } = null!;
    }
}
