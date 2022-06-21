using AutoMapper;
using backaramis.Helpers;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backaramis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StockController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IGenericService<Producto> _stockGenService;
        private readonly IGenericService<ProductoIva> _ivaGenService;
        private readonly IGenericService<ProductoRubro> _rubroGenService;
        private readonly IMapper _mapper;
        private readonly ILoggService _loggService;
        private readonly SecurityService _securityService;
        private readonly string _userName;
        public StockController(
            IStockService stockService,
            IGenericService<Producto> stockGenService,
            IGenericService<ProductoIva> ivaGenService,
            IGenericService<ProductoRubro> rubroGenService,
            IMapper mapper,
            ILoggService loggService,
            SecurityService securityService
            )
        {
            _stockService = stockService;
            _stockGenService = stockGenService;
            _ivaGenService = ivaGenService;
            _rubroGenService = rubroGenService;
            _mapper = mapper;
            _loggService = loggService;
            _securityService = securityService;
            _userName = _securityService.GetUserAuthenticated();
        }

        [HttpGet("GetProductos")]
        public IActionResult GetProductos()
        {
            try
            {
                IEnumerable<ProductoDto>? data = _stockService.GetProductos();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("InsertProducto")]
        public IActionResult InsertProducto([FromForm] ProductoInsert model)
        {
            // map model to entity
            Producto? producto = _mapper.Map<Producto>(model);

            try
            {
                // create user
                _stockGenService.Add(producto);
                _loggService.Log($"InsertProducto {model.Detalle}", "Stock", "Add", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error InsertProducto {model.Detalle}", "Stock", "Add", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteProducto")]
        public IActionResult DeleteProducto(long id)
        {

            try
            {
                // create user
                _stockGenService.Delete(id);
                _loggService.Log($"DeleteProducto {id}", "Stock", "Delete", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error DeleteProducto {id}", "Stock", "Delete", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("UpdateProducto")]
        public IActionResult UpdateProducto([FromBody] List<ProductoUpdate> model)
        {
            var producto = _mapper.Map<List<ProductoUpdate>,List<Producto>>(model);
            try
            {
                // create user
                _stockGenService.Update(producto);
                _loggService.Log($"UpdateProducto {model.First().Detalle}", "Stock", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error UpdateProducto {model.First().Detalle}", "Stock", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        #region Auxiliares 
        [HttpGet]
        [Route("GetIvas")]
        public IActionResult GetIvas()
        {
            List<ProductoIva>? data = _ivaGenService.Get();
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRubros")]
        public IActionResult GetRubros()
        {
            List<ProductoRubro>? data = _rubroGenService.Get();
            return Ok(data);
        }

        [HttpPost("InsertRubro")]
        public IActionResult InsertRubro([FromForm] RubroDto model)
        {
            ProductoRubro? productoRubro = _mapper.Map<ProductoRubro>(model);
            try
            {
                _rubroGenService.Add(productoRubro);
                _loggService.Log($"InsertRubro {model.Detalle}", "Rubro", "Add", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error InsertRubro {model.Detalle}", "Rubro", "Add", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteRubro")]
        public IActionResult DeleteRubro(int id)
        {

            try
            {
                _rubroGenService.Delete(id);
                _loggService.Log($"DeleteRubro {id}", "Rubro", "Delete", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error DeleteRubro {id}", "Rubro", "Delete", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("UpdateRubro")]
        public IActionResult UpdateRubro([FromBody] List<RubroDto> model)
        {
            var productoRubro = _mapper.Map<List<RubroDto>,List<ProductoRubro>>(model);
            try
            {
                _rubroGenService.Update(productoRubro);
                _loggService.Log($"UpdateRubro {model.First().Detalle}", "Rubro", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error UpdateRubro {model.First().Detalle}", "Rubro", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }
}
