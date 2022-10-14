using backaramis.Interfaces;
using backaramis.Models;

namespace backaramis.Services
{
    public class UserService : IUserService
    {
        private readonly AramisContext _context;

        public UserService(AramisContext context)
        {
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null!;
            }

            User? user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null || user.Confirmado == false)
            {
                return null!;
            }

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null!;
            }

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(long id)
        {
            return _context.Users.Find(id)!;
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Falta la contraseña");
            }

            if (_context.Users.Any(x => x.Username == user.Username))
            {
                throw new Exception("El usuario\"" + user.Username + "\" ya está en uso");
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Perfil = 1;
            user.EndOfLife = DateTime.Today.AddDays(-1);
            user.Confirmado = false;
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string? password = null!)
        {
            User? user = _context.Users.Find(userParam.Id);

            if (user == null)
            {
                throw new Exception("No encuentro ese usuario");
            }

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                {
                    throw new Exception("El usuario " + userParam.Username + " ya está en uso");
                }

                user.Username = userParam.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
            {
                user.FirstName = userParam.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
            {
                user.LastName = userParam.LastName;
            }

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            user.Confirmado = userParam.Confirmado;
            user.EndOfLife = userParam.EndOfLife;
            user.Perfil = userParam.Perfil;


            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            User? user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("El valor no puede ser sólo una cadena vacía.", nameof(password));
            }

            using System.Security.Cryptography.HMACSHA512? hmac = new();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("El valor no puede ser sólo una cadena vacía.", nameof(password));
            }

            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Longitud inválida del Password.", nameof(password));
            }

            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Longitud ínválida del Password.", nameof(storedSalt));
            }

            using (System.Security.Cryptography.HMACSHA512? hmac = new(storedSalt))
            {
                byte[]? computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public User ChangePassword(string username, string password, string npassword)
        {
            User? user = Authenticate(username, password);

            if (!string.IsNullOrWhiteSpace(npassword))
            {
                CreatePasswordHash(npassword, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.EndOfLife = DateTime.Today.AddMonths(1);
            }

            _context.Users.Update(user);
            _context.SaveChanges();

            return user;

        }
    }
}