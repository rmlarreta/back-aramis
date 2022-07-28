using AutoMapper;
using backaramis.Helpers;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backaramis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IGenericService<DocumentoDetalle> _genericDelService;
        private readonly IGenericService<Documento> _genericDocService;
        private readonly ILoggService _loggService;
        private readonly IMapper _mapper;
        private readonly SecurityService _securityService;
        private readonly string _userName;
        public DocumentsController(
            IDocumentService documentService,
            IGenericService<DocumentoDetalle> genericDelService,
            IGenericService<Documento> genericDocService,
            ILoggService loggService,
            IMapper mapper,
            SecurityService securityService
            )
        {
            _documentService = documentService;
            _genericDelService = genericDelService;
            _genericDocService = genericDocService;
            _loggService = loggService;
            _mapper = mapper;
            _securityService = securityService;
            _userName = _securityService.GetUserAuthenticated();
        }


        [HttpGet("GetDocuments")]
        public IActionResult GetDocuments()
        {
            try
            {
                Documents? data = _documentService.GetDocuments(null,null,null);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetDocuments/{id}")]
        public IActionResult GetDocuments(long id)
        {
            try
            {
                Documents? data = _documentService.GetDocuments(id,null,null);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetDocumentsByTipo/{tipo}/{estado:int?}")]
        public IActionResult GetDocumentsByTipo(int tipo,int estado=1)
        {
            try
            {
                Documents? data = _documentService.GetDocuments(null, tipo, estado); 
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("InsertDocument")]
        public IActionResult InsertDocument()
        {
            try
            {
                _documentService.InsertDocument(_userName);
                _loggService.Log($"InsertDocument", "Operaciones", "Add", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error InsertDocument", "Operaciones", "Add", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("UpdateDocument")]
        public IActionResult UpdateDocument([FromBody] List<DocumentsUpdateDto> model)
        {
            foreach (var item in model)
            {
                item.Operador = _userName;
                item.Creado = DateTime.Now;
            }
            var data = _mapper.Map<List<DocumentsUpdateDto>, List<Documento>>(model);
            try
            {
                _genericDocService.Update(data);
                _loggService.Log($"UpdateDocument {model.First().Id}", "Documentos", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"ErrorUpdateDocument {model.First().Id}", "Documentos", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("InsertOrden/{id}")]
        public IActionResult InsertOrden(long id)
        {
             try
            {
                var data=_documentService.InsertOrden(id);
                _loggService.Log($"InsertOrden {data.Numero}", "Orden", "Insert", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"InsertOrden {id}", "Orden", "Insert", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("UpdateClienteDocument/{id}/{cliente}")]
        public IActionResult UpdateClienteDocument(long id,long cliente)
        {
            try
            {
                var data = _documentService.UpdateClienteDocument(id,cliente);
                _loggService.Log($"UpdateClienteDocument {id} {cliente}", "Document", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"UpdateClienteDocument {id} {cliente}", "Document", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteDocument/{id}")]
        public IActionResult DeleteDocument(long id)
        {
            try
            {
                _genericDocService.Delete(id);
                _loggService.Log($"DeleteDocument {id}", "Operaciones", "Delete", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error DeleteDocument {id}", "Operaciones", "Delete", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("InsertDetall")]
        public IActionResult InsertDetall([FromBody] List<DocumentsDetallInsertDto> model)

        {
            try
            {
                var data = _mapper.Map<List<DocumentsDetallInsertDto>, List<DocumentoDetalle>>(model);
                _genericDelService.Add(data);
                _loggService.Log($"InsertDetall {model.First().Detalle} en {model.First().Documento}", "Operaciones", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error InsertDetall {model.First().Detalle} en {model.First().Documento}", "Operaciones", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteDetall/{id}")]
        public IActionResult DeleteDetall(long id)
        {
            try
            {
                _genericDelService.Delete(id);
                _loggService.Log($"DeleteDetall", "Operaciones", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error DeleteDetall", "Operaciones", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("UpdateDetall")]
        public IActionResult UpdateDetall([FromBody] List<DocumentsDetallInsertDto> model)
        {
            var data = _mapper.Map<List<DocumentsDetallInsertDto>, List<DocumentoDetalle>>(model);
            try
            {
                _genericDelService.Update(data);
                _loggService.Log($"UpdateDetall {model.First().Detalle} en {model.First().Documento}", "Operaciones", "Update", _userName);
                return Ok("Correcto");
            }
            catch (Exception ex)
            {
                _loggService.Log($"Error UpdateDetall {model.First().Detalle} en {model.First().Documento}", "Operaciones", "Update", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetPdf")]
        [AllowAnonymous]
        public IActionResult PhysicalLocation()
            {
                string physicalPath = "https://core.ac.uk/download/pdf/250239029.pdf";
                byte[] pdfBytes = System.IO.File.ReadAllBytes(physicalPath);
                MemoryStream ms = new MemoryStream(pdfBytes);
                return new FileStreamResult(ms, "application/pdf");
            } 
    }
}
