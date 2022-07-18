﻿using AutoMapper;
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

        public async Task<CancelIntentPayDto> CancelPaymentIntent(string paymentIntent, int id)
        {
            try
            {
                var point = _context.Points.Find(id);
                if (point == null)
                {
                    throw new Exception("Verifique, no existen dispositivos asociados");
                }
                using (var httpClient = new HttpClient())
                {
                    using var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"https://api.mercadopago.com/point/integration-api/devices/{point.DeviceId}/payment-intents/{paymentIntent}");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox");
                    using var response = await httpClient.SendAsync(request);
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

        public async Task<PaymentIntentResponeDto> CreatePaymentIntent(PaymentIntentDto PaymentIntent, int id)
        {
            try
            {
                var point = _context.Points.Find(id);
                if (point == null)
                {
                    throw new Exception("Verifique, no existen dispositivos asociados");
                }
                using (var httpClient = new HttpClient())
                {
                    using var request = new HttpRequestMessage(new HttpMethod("POST"), $"https://api.mercadopago.com/point/integration-api/devices/{point.DeviceId}/payment-intents");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox");
                    request.Content = JsonContent.Create(PaymentIntent);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    using var response = await httpClient.SendAsync(request);
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

        public async Task<StateIntentPayDto> StatePaymentIntent(string paymentIntentId, int id)
        {
            try
            {
                var point = _context.Points.Find(id);
                if (point == null)
                {
                    throw new Exception("Verifique, no existen dispositivos asociados");
                }
                using (var httpClient = new HttpClient())
                {
                    using var request = new HttpRequestMessage(new HttpMethod("GET"), $"https://api.mercadopago.com/point/integration-api/payment-intents/{paymentIntentId}/events");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox");
                    using var response = await httpClient.SendAsync(request);
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
    }
}
