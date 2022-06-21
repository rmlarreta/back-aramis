using backaramis.Models;
using backaramis.Modelsdtos.Documents;

namespace backaramis.Interfaces
{
    public interface IDocumentService
    { 
        Documents GetDocuments(long? Id, int? tipo);
        void InsertDocument(String Operador);
        Documento InsertOrden(long Id);
        Documento UpdateClienteDocument(long Id,long cliente);
    }
}
