using AutoMapper;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Recibos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace backaramis.Services
{

    public class RecibosService : IRecibosService
    {
        private readonly aramisContext _context;
        private readonly IMapper _mapper;
        public RecibosService(aramisContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<CancelIntentPayDto> CancelPaymentIntent(CancelIntentPayDto PaymentIntent)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentIntentDto> CreatePaymentIntent(PaymentIntentDto PaymentIntent)
        {
            Point point = new();
            try
            {


                point =  _context.Points.First();
                PaymentIntent.DeviceId = point.Id;


            }
            catch (Exception)
            {

                throw;
            }
            try
            {
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(new HttpMethod("POST"), $"https://api.mercadopago.com/point/integration-api/devices/{point.Id}/payment-intents");
               
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    var serializer = JsonSerializer.Serialize<PaymentIntentDto>(PaymentIntent);

                    request.Content = new StringContent(serializer);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);
         
            
            return PaymentIntent;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ReciboDto> Insert(ReciboInsertDto ReciboInsert)
        {
            try
            {
                var systemOption = await _context.SystemOptions.FirstOrDefaultAsync();
                if (systemOption == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }

                systemOption.R += 1; //numero de recibo

                foreach (var dr in ReciboInsert.ReciboDetalles)
                {
                    dr.Recibo = systemOption.R;
                }

                var Recibo = _mapper.Map<ReciboInsertDto, Recibo>(ReciboInsert);
                Recibo.Id = systemOption.R;

                var DetallesRecibo = _mapper.Map<List<ReciboDetalleInsertDto>, List<ReciboDetalle>>(ReciboInsert.ReciboDetalles.ToList());

                decimal Total = 0;

                foreach (var d in DetallesRecibo)
                {
                    Total += d.Monto;
                }


                _context.SystemOptions.Update(systemOption);

                await _context.Recibos.AddAsync(Recibo);

                await _context.ReciboDetalles.AddRangeAsync(DetallesRecibo);

                foreach (var doc in ReciboInsert.Documents)
                {
                    _context.Documentos.First(d => d.Id == doc.Id).Recibo = Recibo.Id;
                }

                var result = await _context.SaveChangesAsync();

                var cliente = await _context.Clientes.FirstAsync(x => x.Id == Recibo.Cliente);

                var detalles = await (from det in _context.ReciboDetalles
                                      where det.Recibo == Recibo.Id
                                      select det).ToListAsync();

                var detallesDto = _mapper.Map<List<ReciboDetalle>, List<ReciboDetallDto>>(detalles);

                var reciboDto = new ReciboDto
                {
                    Cliente = cliente.Id,
                    Cuit = cliente.Cuit,
                    Nombre = cliente.Nombre,
                    Fecha = Recibo.Fecha,
                    Operador = Recibo.Operador,
                    Total = Math.Round(Total, 2),
                    Id = Recibo.Id,
                    Detalles = detallesDto,
                    Documents = ReciboInsert.Documents.ToList()

                };


                return reciboDto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public async Task<object> MP()
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.mercadopago.com/point/integration-api/devices/:INGENICO_MOVE2500__ING-ARG-4567446831/payment-intents");
            request.Content = new StringContent("{\n    \"amount\": 1500,\n    \"additional_info\": {\n        \"external_reference\": \"4561ads-das4das4-das4754-das456\",\n        \"print_on_terminal\": true,\n        \"ticket_number\": \"S0392JED\"\n    }\n}");
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer TEST-1288734920192107-062019-24a4126adc8ae98f34e36c9347f1c68b-1146429935");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                // perhaps check some headers before deserialising

                try
                {
                    var json= await response.Content.ReadFromJsonAsync<object>();
                    return json;
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

            return null;
        }

        public Task<StateIntentPayDto> StatePaymentIntent(PaymentIntentDto StateIntentPayDto)
        {
            throw new NotImplementedException();
        }
    }
}
