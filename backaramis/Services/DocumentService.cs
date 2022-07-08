using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Documents;
using Microsoft.Data.SqlClient;
using System.Data;

namespace backaramis.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IStoreProcedure _storeProcedure;
        private readonly aramisContext _context;

        public DocumentService(IStoreProcedure storeProcedure, aramisContext context)
        {
            _storeProcedure = storeProcedure;
            _context = context;
        }
        public Documents GetDocuments(long? Id = null, int? tipo = null)
        {

            try
            {
                DataSet ds = new();
                if (Id == null && tipo == null)
                {
                    ds = _storeProcedure.SpWhithDataSetPure("DocumentoGet");
                }
                else
                {
                    List<SqlParameter> Params = new();
                    if (Id != null)
                    {
                        Params.Add(new SqlParameter("@Id", Id));
                    }
                    if (tipo != null)
                    {
                        Params.Add(new SqlParameter("@tipo", tipo));
                    }
                    ds = _storeProcedure.SpWhithDataSetPure("DocumentoGet", Params);
                }
                List<DocumentsDto> documents = new();
                List<DocumentsDetallDto> detalles = new();
                DataTable dtDocuments = new();
                DataTable dtDetalles = new();
                dtDocuments = ds.Tables[0];
                dtDetalles = ds.Tables[1];

                foreach (DataRow row in dtDocuments.Rows)
                {
                    documents.Add(new DocumentsDto()
                    {
                        Id = (long)row["Id"],
                        Cuit = (long)row["Cuit"],
                        Fecha = (DateTime)row["Fecha"],
                        Letra = row["Letra"].ToString(),
                        Nombre = row["Nombre"].ToString(),
                        Cliente = (long)row["Cliente"],
                        Numero = (int)row["Numero"],
                        Observaciones = row["Observaciones"].ToString(),
                        Operador = row["Operador"].ToString(),
                        Pos = (int)row["Pos"],
                        CodTipo = (int)row["CodTipo"],
                        Tipo = row["Tipo"].ToString(),
                        Total = (decimal)row["Total"],
                        Limite = (decimal)row["Limite"]
                    });
                }

                foreach (DataRow rowQ in dtDetalles.Rows)
                {
                    detalles.Add(new DocumentsDetallDto()
                    {
                        Cantidad = (decimal)rowQ["Cantidad"],
                        Detalle = rowQ["Detalle"].ToString(),
                        Documento = (long)rowQ["Documento"],
                        Id = (long)rowQ["Id"],
                        Internos = (decimal)rowQ["Internos"],
                        Iva = (decimal)rowQ["Iva"],
                        Unitario = (decimal)rowQ["Unitario"],
                        Codigo = rowQ["Codigo"].ToString(),
                        Producto = (long)rowQ["Producto"],
                        Rubro = rowQ["Rubro"].ToString(),
                        SubTotal = (decimal)rowQ["SubTotal"]
                    });
                }
                Documents data = new();
                data.DocumentsDto = documents;
                data.DocumentsDetallDto = detalles;
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public void InsertDocument(string Operador)
        {
            try
            {
                List<SqlParameter> Params = new();
                Params.Add(new SqlParameter("@operador", Operador));
                _storeProcedure.ExecuteNonQuery("DocumentInsert", Params);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public Documento InsertOrden(long Id)
        {
            try
            {
                var tipo = _context.DocumentoTipos.FirstOrDefault(x => x.Letra == "O");
                var numero = _context.SystemOptions.FirstOrDefault();
                if (numero == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }
                numero.O += 1;
                if (tipo == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }
                var document = _context.Documentos.FirstOrDefault(x => x.Id == Id);
                if (document == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }
                document.Tipo = tipo.Id;
                document.Fecha = DateTime.Now;
                document.Creado = DateTime.Now;
                document.Numero = numero.O;
                _context.SystemOptions.UpdateRange(numero);
                _context.Documentos.Update(document);
                _context.SaveChanges();
                return document;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public Documento UpdateClienteDocument(long Id, long cliente)
        {
            try
            {
                var document = _context.Documentos.FirstOrDefault(x => x.Id == Id);
                var persona = _context.Clientes.First(x => x.Id == cliente);
                if (document == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }

                document.Cliente = cliente;
                document.Razon = persona.Nombre; 
                _context.SaveChanges();
                return document;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
