using AutoMapper;
using backaramis.Helpers;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backaramis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILoggService _loggService;
        private readonly IGenericService<UserPerfil> _perfilesServices;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly SecurityService _securityService;
        private readonly string _userName;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            ILoggService loggService,
            IOptions<AppSettings> appSettings,
            SecurityService securityService,
            IGenericService<UserPerfil> perfilesServices

            )
        {
            _perfilesServices = perfilesServices;
            _userService = userService;
            _loggService = loggService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _securityService = securityService;
            _userName = _securityService.GetUserAuthenticated();
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromForm] Login model)
        {
            User? user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                _loggService.Log("Logueo Incorrecto", "Users", "Loguin", model.Username);
                return BadRequest(new { message = "Logueo Incorrecto" });
            }

            if (user.EndOfLife < DateTime.Now.AddHours(-3))
            {
                _loggService.Log("Logueo Incorrecto. Clave Vencida", "Users", "Loguin", model.Username);
                return BadRequest(new { message = user.FirstName + " debes renovar tus datos" });
            }

            JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
            byte[]? key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            SecurityTokenDescriptor? tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                     new Claim(ClaimTypes.NameIdentifier, user.FirstName.ToString()),
                          new Claim(ClaimTypes.GivenName, user.LastName.ToString()),
                       new Claim(ClaimTypes.Role, user.Perfil.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
            string? tokenString = tokenHandler.WriteToken(token);
            _loggService.Log("Logueo Correcto", "Users", "Loguin", model.Username);
            // return basic user info and authentication token
            return Ok(new
            {
                user.Id,
                user.Username,
                user.FirstName,
                user.LastName,
                user.Perfil,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromForm] AddUser model)
        {
            // map model to entity
            User? user = _mapper.Map<User>(model);

            try
            {
                // create user
                _userService.Create(user, model.Password);
                _loggService.Log("Usuario Creado", "Users", "Add", model.Username);
                return Ok("Solicite la confirmación de su nuevo usuario");
            }
            catch (Exception ex)
            {
                _loggService.Log("Error tratando de crear usuario", "Users", "Add", model.Username);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("changepassword")]
        public IActionResult ChangePassword([FromForm] ChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                _loggService.Log("Logueo y cambio Pass Incorrecto", "Users", "Loguin", model.Username);
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            if (model.Password == model.NPassword)
            {
                _loggService.Log("Repite Pass", "Users", "Loguin", model.Username);
                return BadRequest(new { message = "La contraseña no puede ser igual a la anterior" });
            }

            User? user = _userService.ChangePassword(model.Username, model.Password, model.NPassword);

            if (user == null)
            {
                _loggService.Log("Logueo y cambio Pass Incorrecto", "Users", "Loguin", model.Username);
                return BadRequest(new { message = "Logueo Incorrecto" });
            }

            if (user.EndOfLife < DateTime.Now.AddHours(-3))
            {
                _loggService.Log("Logueo y cambio Pass Incorrecto. Clave Vencida", "Users", "Loguin", model.Username);
                return BadRequest(new { message = user.FirstName + " debes renovar tus datos" });
            }

            JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
            byte[]? key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            SecurityTokenDescriptor? tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                     new Claim(ClaimTypes.NameIdentifier, user.FirstName.ToString()),
                          new Claim(ClaimTypes.GivenName, user.LastName.ToString()),
                       new Claim(ClaimTypes.Role, user.Perfil.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
            string? tokenString = tokenHandler.WriteToken(token);
            _loggService.Log("Logueo y Cambio Pass Correcto", "Users", "Loguin", model.Username);
            // return basic user info and authentication token
            return Ok(new
            {
                user.Id,
                user.Username,
                user.FirstName,
                user.LastName,
                user.Perfil,
                Token = tokenString
            });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<User>? users = _userService.GetAll();
            IList<UserModel>? data = _mapper.Map<IList<UserModel>>(users);
            return Ok(data);
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(long id)
        {
            try
            {
                _userService.Delete(id);
                _loggService.Log($"Usuario {id} Eliminado", "Users", "Delete", _userName);
                return Ok(new { message = "Usuario Eliminado" });
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error tratando de Eliminar Usuario  {id}", "Users", "Delete", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.InnerException is not null ? ex.InnerException.Message : ex.Message });
            }
        }

        [HttpPatch]
        [Route("UpdateUser")]
        public IActionResult UpdateUser([FromForm] EditUser model)
        {

            try
            {
                User? user = _mapper.Map<User>(model);
                if (string.IsNullOrWhiteSpace(model.Password) || model.Password == "undefined")
                {
                    _userService.Update(user);
                }
                else
                {
                    _userService.Update(user, model.Password);
                }

                _loggService.Log($"Usuario {model.Username} Actualizado", "Users", "Update", _userName);
                return Ok(new { message = $"Usuario {model.Username} Actualizado" });
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error tratando de Actualizar Usuario {model.Username}", "Users", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.InnerException is not null ? ex.InnerException.Message : ex.Message });
            }
        }

        #region Auxiliares
        [HttpGet]
        [Route("GetPerfiles")]
        public IActionResult GetPerfiles()
        {
            List<UserPerfil>? perfiles = _perfilesServices.Get();
            List<PerfilModel>? data = _mapper.Map<List<UserPerfil>, List<PerfilModel>>(perfiles);
            return Ok(data);
        }
        #endregion
    }
}
