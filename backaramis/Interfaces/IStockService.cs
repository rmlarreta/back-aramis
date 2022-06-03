using backaramis.Modelsdtos.Stock;

namespace backaramis.Interfaces
{
    public interface IStockService
    {
        IEnumerable<ProductoDto> GetProductos();
    }
}
