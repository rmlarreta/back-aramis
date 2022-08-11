﻿using backaramis.Models;
using backaramis.Modelsdtos.Documents;
using Microsoft.AspNetCore.Mvc;

namespace backaramis.Interfaces
{
    public interface IDocumentService
    {
        Documents GetDocuments(long? Id, int? tipo, int? estado);
        void InsertDocument(string Operador);
        Documento InsertOrden(long Id);
        Documento UpdateClienteDocument(long Id, long cliente);
        FileStreamResult ReporteRemito(int id);
    }
}
