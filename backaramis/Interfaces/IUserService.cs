using backaramis.Models;

namespace backaramis.Interfaces
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User ChangePassword(string username, string password, string npassword);
        IEnumerable<User> GetAll();
        User GetById(long id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(long id);
    }
}
