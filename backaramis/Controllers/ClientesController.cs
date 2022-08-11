using AutoMapper;
using backaramis.Helpers;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Clientes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backaramis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ClientesController : Controller
    {
        private readonly IGenericService<Cliente> _clienteGenService;
        private readonly IGenericService<ClienteResponsabilidad> _resposGenService;
        private readonly IGenericService<ClienteGenero> _generoGenService;
        private readonly IGenericService<ProveedorImputacion> _imputacionGenService;
        private readonly IMapper _mapper;
        private readonly ILoggService _loggService;
        private readonly SecurityService _securityService;
        private readonly string _userName;
        public ClientesController(
            IGenericService<Cliente> clienteGenService,
            IGenericService<ClienteResponsabilidad> resposGenService,
            IGenericService<ClienteGenero> generoGenService,
            IGenericService<ProveedorImputacion> imputacionGenService,
            IMapper mapper,
            ILoggService loggService,
            SecurityService securityService
            )
        {
            _clienteGenService = clienteGenService;
            _resposGenService = resposGenService;
            _generoGenService = generoGenService;
            _imputacionGenService = imputacionGenService;
            _mapper = mapper;
            _loggService = loggService;
            _securityService = securityService;
            _userName = _securityService.GetUserAuthenticated();
        }

        [HttpGet("GetClientes")]
        public IActionResult GetClientes()
        {
            try
            {
                List<Cliente>? clientes = _clienteGenService.Get();
                //List<ClienteDto> data = new();
                List<ClienteDto>? data = _mapper.Map<List<Cliente>, List<ClienteDto>>(clientes);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("InsertCliente")]
        public IActionResult InsertCliente([FromForm] ClienteInsert model)
        {
            // map model to entity
            Cliente? cliente = _mapper.Map<Cliente>(model);

            try
            {
                // create user
                _clienteGenService.Add(cliente);
                _loggService.Log($"InsertCliente {model.Nombre}", "Clientes", "Add", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error InsertCliente {model.Nombre}", "Clientes", "Add", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("UpdateCLiente")]
        public IActionResult UpdateCLiente([FromBody] List<ClienteDto> model)
        {
            List<Cliente>? cliente = _mapper.Map<List<ClienteDto>, List<Cliente>>(model);
            try
            {
                // create user
                _clienteGenService.Update(cliente);
                _loggService.Log($"UpdateCLiente {model.First().Nombre}", "Clientes", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error UpdateCLiente {model.First().Nombre}", "Clientes", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteCliente")]
        public IActionResult DeleteCliente(long id)
        {
            try
            {
                // create user
                _clienteGenService.Delete(id);
                _loggService.Log($"DeleteCliente {id}", "Clientes", "Delete", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error DeleteCliente {id}", "Clientes", "Delete", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        #region Auxiliares 
        [HttpGet]
        [Route("GetRespo")]
        public IActionResult GetRespo()
        {
            List<ClienteResponsabilidad>? data = _resposGenService.Get();
            return Ok(data);
        }

        [HttpGet]
        [Route("GetGenero")]
        public IActionResult GetGenero()
        {
            IEnumerable<ClienteGenero>? data = _generoGenService.Get();
            return Ok(data);
        }

        [HttpGet]
        [Route("GetImputaciones")]
        public IActionResult GetImputaciones()
        {
            List<ProveedorImputacion>? data = _imputacionGenService.Get();
            return Ok(data);
        }

        [HttpPost("InsertImputaciones")]
        public IActionResult InsertImputaciones([FromForm] ProveedorImputacion model)
        {
            try
            {
                // create user
                _imputacionGenService.Add(model);
                _loggService.Log($"InsertImputaciones {model.Id}", "ProveedorImputacion", "Add", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error InsertImputaciones {model.Id}", "ProveedorImputacion", "Add", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("UpdateImputaciones")]
        public IActionResult UpdateImputaciones([FromBody] List<ProveedorImputacion> model)
        {
            try
            {
                // create user
                _imputacionGenService.Update(model);
                _loggService.Log($"UpdateImputaciones {model.First().Detalle}", "ProveedorImputacion", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error UpdateImputaciones {model.First().Detalle}", "ProveedorImputacion", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteImputaciones")]
        public IActionResult DeleteImputaciones(long id)
        {
            try
            {
                // create user
                _imputacionGenService.Delete(id);
                _loggService.Log($"DeleteImputaciones {id}", "ProveedorImputacion", "Delete", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error DeleteImputaciones {id}", "ProveedorImputacion", "Delete", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }
}
