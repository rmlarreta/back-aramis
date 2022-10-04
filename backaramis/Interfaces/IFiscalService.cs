using AfipServiceReference;
using backaramis.Models;
using backaramis.Modelsdtos.Documents;

namespace backaramis.Interfaces
{
    public interface IFiscalService
    {
        public Task<FECAESolicitarResponse> GetCae(DocumentoFiscal documento);
        public Task<long> FacturaRemito(Documento documento);
    }
}
