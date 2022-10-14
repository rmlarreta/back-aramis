namespace backaramis.Models
{
    public partial class UserModule
    {
        public UserModule()
        {
            UserOperations = new HashSet<UserOperation>();
        }

        public int Id { get; set; }
        public string Modulo { get; set; } = null!;

        public virtual ICollection<UserOperation> UserOperations { get; set; }
    }
}
