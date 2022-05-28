using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Stock;
using Microsoft.Data.SqlClient;
using System.Data;
namespace backmaree.Services
{
    public class StockService : IStockService
    {
        private readonly IStoreProcedure _storeProcedure;
        private readonly aramisContext _context;

        public StockService(IStoreProcedure storeProcedure, aramisContext context)
        {
            _storeProcedure = storeProcedure;
            _context = context;
        }

        public IEnumerable<ProductoDto> GetProductos()
        {
            try
            {
                DataSet ds = _storeProcedure.SpWhithDataSetPure("ProductosGet");
                List<ProductoDto> productos = new(); 
                DataTable dtProductos = new(); 
                dtProductos = ds.Tables[0]; 
                foreach (DataRow row in dtProductos.Rows)
                {
                    productos.Add(new ProductoDto()
                    {
                        Id = (long)row["Id"],
                        Codigo = row["Codigo"].ToString(),
                        Detalle = row["Detalle"].ToString(), 
                        Rubro = (int)row["Rubro"],
                        RubroStr = row["RubroStr"].ToString(),
                        Costo = (decimal)row["Costo"],
                        Iva = (int)row["Iva"],
                        IvaDec = (decimal)row["IvaDec"],
                        Internos = (decimal)row["Internos"],
                        Tasa = (decimal)row["Tasa"],
                        Stock = (decimal)row["Stock"],
                        Servicio = (bool)row["Servicio"],
                        Precio = (decimal)row["Precio"]
                    });
                }
                  
                return productos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
    }
}
