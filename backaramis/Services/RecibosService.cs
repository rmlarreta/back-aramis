using AutoMapper;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Documents;
using backaramis.Modelsdtos.Recibos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;

namespace backaramis.Services
{

    public class RecibosService : IRecibosService
    {
        private readonly aramisContext _context;
        private readonly IMapper _mapper;
        private readonly IStoreProcedure _storeProcedure;
        public RecibosService(aramisContext context, IMapper mapper, IStoreProcedure storeProcedure)
        {
            _context = context;
            _mapper = mapper;
            _storeProcedure = storeProcedure;
        }
        public async Task<PaymentIntentResponeDto> CreatePaymentIntent(PaymentIntentDto PaymentIntent, int id)
        {
            try
            {
                await Getpaymentintentlist(id);
                Point? point = _context.Points.Find(id);
                if (point == null)
                {
                    throw new Exception("Verifique, no existen dispositivos asociados");
                }
                using (HttpClient? httpClient = new HttpClient())
                {
                    using HttpRequestMessage? request = new HttpRequestMessage(new HttpMethod("POST"), $"https://api.mercadopago.com/point/integration-api/devices/{point.DeviceId}/payment-intents");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox"); //borrar en produccion
                    request.Content = JsonContent.Create(PaymentIntent);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    using HttpResponseMessage? response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return await response.Content.ReadFromJsonAsync<PaymentIntentResponeDto>();

                        }
                        catch (NotSupportedException) // When content type is not valid
                        {
                            Console.WriteLine("The content type is not supported.");
                        }
                        catch (JsonException) // Invalid JSON
                        {
                            Console.WriteLine("Invalid JSON.");
                        }
                    }
                }
                return null;
            }

            catch (Exception ex)
            {

                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
        public async Task Getpaymentintentlist(int id)
        {
            try
            {
                Point? point = _context.Points.Find(id);
                if (point == null)
                {
                    throw new Exception("Verifique, no existen dispositivos asociados");
                }
                using (HttpClient? httpClient = new HttpClient())
                {
                    using HttpRequestMessage? request = new HttpRequestMessage(new HttpMethod("GET"), $"https://api.mercadopago.com/point/integration-api/payment-intents/events?startDate={DateTime.Today.ToString("yyyy-MM-dd")}&endDate={DateTime.Today.ToString("yyyy-MM-dd")}");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox"); //borrar en produccion
                    using HttpResponseMessage? response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            EventoDto? events = await response.Content.ReadFromJsonAsync<EventoDto>();
                            foreach (Evento? evento in events.events)
                            {
                                if (evento.status == "OPEN")
                                {
                                    await CancelPaymentIntent(evento.payment_intent_id, id);
                                }
                            }

                        }
                        catch (NotSupportedException) // When content type is not valid
                        {
                            Console.WriteLine("The content type is not supported.");
                        }
                        catch (JsonException) // Invalid JSON
                        {
                            Console.WriteLine("Invalid JSON.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
        public async Task<CancelIntentPayDto> CancelPaymentIntent(string paymentIntent, int id)
        {
            try
            {
                Point? point = _context.Points.Find(id);
                if (point == null)
                {
                    throw new Exception("Verifique, no existen dispositivos asociados");
                }
                using (HttpClient? httpClient = new HttpClient())
                {
                    using HttpRequestMessage? request = new HttpRequestMessage(new HttpMethod("DELETE"), $"https://api.mercadopago.com/point/integration-api/devices/{point.DeviceId}/payment-intents/{paymentIntent}");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox"); //borrar en produccion
                    using HttpResponseMessage? response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return await response.Content.ReadFromJsonAsync<CancelIntentPayDto>();

                        }
                        catch (NotSupportedException) // When content type is not valid
                        {
                            Console.WriteLine("The content type is not supported.");
                        }
                        catch (JsonException) // Invalid JSON
                        {
                            Console.WriteLine("Invalid JSON.");
                        }
                    }
                }
                return null;
            }

            catch (Exception ex)
            {

                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
        public async Task<StateIntentPayDto> StatePaymentIntent(string paymentIntentId, int id)
        {
            try
            {
                Point? point = _context.Points.Find(id);
                if (point == null)
                {
                    throw new Exception("Verifique, no existen dispositivos asociados");
                }
                using (HttpClient? httpClient = new HttpClient())
                {
                    using HttpRequestMessage? request = new HttpRequestMessage(new HttpMethod("GET"), $"https://api.mercadopago.com/point/integration-api/payment-intents/{paymentIntentId}/events");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox"); //borrar en produccion
                    using HttpResponseMessage? response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return await response.Content.ReadFromJsonAsync<StateIntentPayDto>();

                        }
                        catch (NotSupportedException) // When content type is not valid
                        {
                            Console.WriteLine("The content type is not supported.");
                        }
                        catch (JsonException) // Invalid JSON
                        {
                            Console.WriteLine("Invalid JSON.");
                        }
                    }
                }
                return null;
            }

            catch (Exception ex)
            {

                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
        public async Task<ReciboDto> Insert(ReciboInsertDto ReciboInsert)
        {
            try
            {
                Recibo? Recibo = _mapper.Map<ReciboInsertDto, Recibo>(ReciboInsert);

                SystemOption? systemOption = await _context.SystemOptions.FirstOrDefaultAsync();
                if (systemOption == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }
                systemOption.R += 1; //numero de recibo
                Recibo.Id = systemOption.R;
                _context.SystemOptions.Attach(systemOption);

                decimal Total = 0;
                decimal PagoAplicado = 0;

                foreach (ReciboDetalle? d in Recibo.ReciboDetalles)
                {
                    Total += d.Monto;
                    d.Recibo = Recibo.Id;
                }

                await _context.Recibos.AddAsync(Recibo);

                foreach (int doc in ReciboInsert.Documents)
                {
                    //cambiamos de update el document a insertar un documento-recibo con el monto
                    Documento? docu = await _context.Documentos.FirstAsync(d => d.Id == doc);
                    IQueryable<decimal>? q = from d in _context.DocumentoDetalles
                                             where d.Documento == doc
                                             select d.Unitario * d.Cantidad;
                    if (Total - PagoAplicado >= q.Sum() - docu.Pago)
                    {
                        docu.Pago = q.Sum();
                        PagoAplicado += q.Sum();
                        DocumentoRecibo documentoRecibo = new()
                        {
                            Documento = docu.Id,
                            Recibo = Recibo.Id,
                            Monto = docu.Pago
                        };
                        _context.DocumentoRecibos.Add(documentoRecibo);
                    }
                    else if (Total - PagoAplicado >= 0)
                    {
                        docu.Pago += Total - PagoAplicado;
                        PagoAplicado = Total;
                        DocumentoRecibo documentoRecibo = new()
                        {
                            Documento = docu.Id,
                            Recibo = Recibo.Id,
                            Monto = Total - PagoAplicado
                        };
                        _context.DocumentoRecibos.Add(documentoRecibo);
                    }
                    _context.Documentos.Update(docu);
                }

                //Manejo del Documento
                if (ReciboInsert.CodTipo != null)
                {
                    Documento? documento = await _context.Documentos.FirstAsync(x => x.Id == ReciboInsert.Documents.First());
                    if (documento != null)
                    {
                        documento.Estado = _context.DocumentoEstados.FirstOrDefaultAsync(d => d.Detalle == "ENTREGADO").Result.Id;
                        //si viene de un presupuesto lo transforma en lo que sea
                        if (documento.Tipo == _context.DocumentoTipos.FirstOrDefault(x => x.Detalle == "PRESUPUESTO").Id)
                        {
                            documento.Tipo = _context.DocumentoTipos.FirstOrDefault(x => x.Detalle == "REMITO").Id;
                        }
                        //si viene de una orden la modifica y crea uno nuevo

                        if (documento.Tipo == _context.DocumentoTipos.FirstOrDefault(x => x.Detalle == "ORDEN DE SERVICIO").Id)
                        {
                            Documento newRemito = new();
                            newRemito = documento;
                            newRemito.Tipo = _context.DocumentoTipos.FirstOrDefault(x => x.Detalle == "REMITO").Id;
                            _context.Documentos.Add(newRemito);
                        }
                    }
                    _context.Documentos.Update(documento);
                }

                int result = await _context.SaveChangesAsync();

                return result > 0 ? GetRecibo(Recibo.Id) : throw new Exception("No se pudo ingresar el Recibo");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
        public ReciboDto GetRecibo(int id)
        {
            try
            {
                DataSet ds = new();

                List<SqlParameter> Params = new();
                if (id != 0)
                {
                    Params.Add(new SqlParameter("@recibo", id));
                }

                ds = _storeProcedure.SpWhithDataSetPure("ReciboDto", Params);
                ReciboDto reciboDto = new();
                List<ReciboDetallDto>? detalles = new();
                List<DocumentsDto>? documents = new();
                DataTable dtDocuments = new();
                DataTable dtDetalles = new();
                DataTable dtRecibo = new();
                dtRecibo = ds.Tables[0];
                dtDetalles = ds.Tables[1];
                dtDocuments = ds.Tables[2];


                foreach (DataRow row in dtDetalles.Rows)
                {
                    detalles.Add(new ReciboDetallDto()
                    {
                        Id = (long)row["Id"],
                        Codigo = row["Codigo"].ToString(),
                        Detalle = row["Detalle"].ToString(),
                        Sucursal = row["Sucursal"].ToString(),
                        Tipo = row["Tipo"].ToString(),
                        Monto = (decimal)row["Monto"]
                    });
                }

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
                        Numero =  row["Numero"].ToString(),
                        Observaciones = row["Observaciones"].ToString(),
                        Operador = row["Operador"].ToString(),
                        Pos = row["Pos"].ToString(),
                        CodTipo = (int)row["CodTipo"],
                        Tipo = row["Tipo"].ToString(),
                        Total = (decimal)row["Total"],
                        Limite = (decimal)row["Limite"],
                        EstadoPago = row["EstadoPago"].ToString()
                    });
                }

                foreach (DataRow row in dtRecibo.Rows)
                {
                    reciboDto.Id = (int)row["Id"];
                    reciboDto.Cliente = (long)row["Cliente"];
                    reciboDto.Cuit = (long)row["Cuit"];
                    reciboDto.Nombre = row["Nombre"].ToString();
                    reciboDto.Fecha = (DateTime)row["Fecha"];
                    reciboDto.Operador = row["Operador"].ToString();
                    reciboDto.Total = (decimal)row["Total"];
                    reciboDto.Documents = documents;
                    reciboDto.Detalles = detalles;
                };

                return reciboDto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
