using AutoMapper;
using backaramis.Helpers;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Recibos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;

namespace backaramis.Services
{

    public class RecibosService : IRecibosService
    {
        private readonly IStoreProcedure _storeProcedure;
        private readonly AramisContext _context;
        private readonly IMapper _mapper;
        public RecibosService(AramisContext context,
                              IStoreProcedure storeProcedure,
                              IMapper mapper)
        {
            _storeProcedure= storeProcedure;
            _context = context;
            _mapper = mapper;
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
                using (HttpClient? httpClient = new())
                {
                    using HttpRequestMessage? request = new(new HttpMethod("POST"), $"https://api.mercadopago.com/point/integration-api/devices/{point.DeviceId}/payment-intents");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox"); //borrar en produccion
                    request.Content = JsonContent.Create(PaymentIntent);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    using HttpResponseMessage? response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            PaymentIntentResponeDto? result = await response.Content.ReadFromJsonAsync<PaymentIntentResponeDto>();
                            return result!;

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
                return null!;
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
                using HttpClient? httpClient = new();
                using HttpRequestMessage? request = new(new HttpMethod("GET"), $"https://api.mercadopago.com/point/integration-api/payment-intents/events?startDate={DateTime.Today:yyyy-MM-dd}&endDate={DateTime.Today:yyyy-MM-dd}");
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox"); //borrar en produccion
                using HttpResponseMessage? response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        EventoDto? events = await response.Content.ReadFromJsonAsync<EventoDto>();
                        foreach (Evento? evento in events!.Events!)
                        {
                            if (evento.Status == "OPEN")
                            {
                                await CancelPaymentIntent(evento.Payment_intent_id!, id);
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
                using (HttpClient? httpClient = new())
                {
                    using HttpRequestMessage? request = new(new HttpMethod("DELETE"), $"https://api.mercadopago.com/point/integration-api/devices/{point.DeviceId}/payment-intents/{paymentIntent}");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox"); //borrar en produccion
                    using HttpResponseMessage? response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            CancelIntentPayDto? result = await response.Content.ReadFromJsonAsync<CancelIntentPayDto>();
                            return result!;
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
                return null!;
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
                using (HttpClient? httpClient = new())
                {
                    using HttpRequestMessage? request = new(new HttpMethod("GET"), $"https://api.mercadopago.com/point/integration-api/payment-intents/{paymentIntentId}/events");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {point.Token}");
                    request.Headers.TryAddWithoutValidation("x-test-scope", "sandbox"); //borrar en produccion
                    using HttpResponseMessage? response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            StateIntentPayDto? result = await response.Content.ReadFromJsonAsync<StateIntentPayDto>();
                            return result!;

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
                return null!;
            }

            catch (Exception ex)
            {

                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
        public async Task<int> Insert(ReciboInsertDto ReciboInsert)
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
                
                decimal Total = Recibo.ReciboDetalles.Sum(x => x.Monto);
                decimal TotalDocumento = 0;
                decimal PagoAplicado = 0;

                await _context.Recibos.AddAsync(Recibo);

                foreach (int doc in ReciboInsert.Documents!)
                {
                    //cambiamos de update el document a insertar un documento-recibo con el monto
                    Documento? docu = await _context.Documentos.FirstAsync(d => d.Id == doc);
                    IQueryable<decimal>? q = from d in _context.DocumentoDetalles
                                             where d.Documento == doc
                                             select d.Unitario * d.Cantidad;
                    TotalDocumento = q.Sum();
                    if (Total - PagoAplicado >= TotalDocumento - docu.Pago)
                    {
                        docu.Pago = TotalDocumento;
                        PagoAplicado += TotalDocumento;
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
                        List<SqlParameter> parameters = new()
                        {
                            new SqlParameter("@documento", documento!.Id)
                        }; 
                        systemOption.X += 1;
                        documento.Estado = _context.DocumentoEstados.FirstOrDefaultAsync(d => d.Detalle == "ENTREGADO").Result!.Id;
                        //si viene de un presupuesto lo transforma en lo que sea
                        if (documento.Tipo == _context.DocumentoTipos.FirstOrDefault(x => x.Detalle == "PRESUPUESTO")!.Id)
                        { 
                            documento.Tipo = _context.DocumentoTipos.FirstOrDefault(x => x.Detalle == "REMITO")!.Id;
                            documento.Fecha = DateTime.Now;
                            documento.Numero = systemOption.X;
                            documento.Razon = _context.Clientes.FirstOrDefault(x=>x.Id==documento.Cliente)!.Nombre;
                            _storeProcedure.SpWhithDataSetPure("DocumentDescuentaStock", parameters);
                        }
                        //si viene de una orden la modifica y crea uno nuevo

                        if (documento.Tipo == _context.DocumentoTipos.FirstOrDefault(x => x.Detalle == "ORDEN DE SERVICIO")!.Id)
                        {
                            DocumentoOrden pasoAremito = new();
                            pasoAremito.Orden = documento.Numero;
                            pasoAremito.Documento = documento.Id;
                            _context.DocumentoOrdens.Add(pasoAremito);
                            documento.Tipo = _context.DocumentoTipos.FirstOrDefault(x => x.Detalle == "REMITO")!.Id;
                            documento.Fecha = DateTime.Now;
                            documento.Numero = systemOption.X;
                            documento.Razon = _context.Clientes.FirstOrDefault(x => x.Id == documento.Cliente)!.Nombre;
                            _storeProcedure.SpWhithDataSetPure("DocumentDescuentaStock", parameters);
                        }
                      
                        _context.SystemOptions.Attach(systemOption);
                        _context.Documentos.Update(documento!);
                    }
                }

                int result = await _context.SaveChangesAsync();

                return result > 0 ? Recibo.Id : throw new Exception("No se pudo ingresar el Recibo");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
        public FileStreamResult GetReciboReport(int id)
        {
            try
            {
                Recibo? recibo = _context.Recibos
                             .Where(r => r.Id == id)
                             .Include(rd => rd.ReciboDetalles)
                             .FirstOrDefault();

                decimal total = 0.0m;
                foreach (ReciboDetalle? rd in recibo!.ReciboDetalles)
                {
                    total += rd.Monto;
                }

                string letras = ExtensionMethods.NumeroLetras(total);

                Cliente? cliente = _context.Clientes
                              .Where(c => c.Id == recibo!.Cliente)
                             .FirstOrDefault();

                SystemOption? empresa = _context.SystemOptions.FirstOrDefault();

                if (recibo == null)
                {
                    return null!;
                }

                MemoryStream? stream = new();

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        //seteamos la página
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(5, Unit.Millimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(20));
                        page.DefaultTextStyle(x => x.FontFamily("Arial"));
                        page.Header()
                        .Grid(grid =>
                        {
                            grid.Columns(11);
                            grid.Item(5)
                              .Column(column =>
                              {
                                  column.Item().Height(35, Unit.Millimetre).Row(row =>
                                  {
                                      row.RelativeItem(1).Image("Images/logo.jpg");

                                      row.RelativeItem(1).Border(1, Unit.Point)
                                     .Grid(grid =>
                                     {
                                         grid.Columns(1);
                                         grid.Item()
                                     .DefaultTextStyle(x => x.FontSize(40))
                                     .DefaultTextStyle(f => f.Bold())
                                     .AlignCenter()
                                     .PaddingVertical(1, Unit.Millimetre)
                                     .Text(text =>
                                     {
                                         text.Span("R");
                                     });
                                         grid.Item()
                                        .DefaultTextStyle(x => x.FontSize(10))
                                        .AlignCenter()
                                        .PaddingBottom(3, Unit.Millimetre)
                                        .Text(text =>
                                        {
                                            text.Span("ORIGINAL");
                                        });
                                     });

                                      row.Spacing(10);
                                      row.RelativeItem(2)
                                      .Border(1, Unit.Point)
                                      .Padding(5, Unit.Millimetre)
                                      .DefaultTextStyle(x => x.FontSize(12))
                                      .DefaultTextStyle(f => f.Bold())
                                      .Grid(grid =>
                                      {
                                          grid.Columns(1);
                                          grid.Item()
                                           .Text(text =>
                                           {
                                               text.AlignCenter();
                                               text.Span("RECIBO");
                                           });
                                          grid.Item()
                                           .Text(text =>
                                           {
                                               text.AlignRight();
                                               text.Span("\n" + "FC: " + recibo.Id.ToString("D10"));
                                           });
                                          grid.Item()
                                          .Text(text =>
                                          {
                                              text.DefaultTextStyle(f => f.FontSize(12));
                                              text.AlignRight();
                                              text.Span("FECHA: " + recibo.Fecha.ToShortDateString());
                                          });
                                      });

                                  });
                                  column.Item().Row(row =>
                                  {
                                      row.RelativeItem(2)
                                      .PaddingVertical(3, Unit.Millimetre)
                                      .DefaultTextStyle(f => f.FontSize(10))
                                      .DefaultTextStyle(t => t.FontColor("#1f66ff"))
                                      .Text("Razón Social: " + empresa!.Razon
                                      + "\n" + empresa!.Fantasia
                                      + "\n" + empresa!.Domicilio
                                      + "\n" + empresa!.ResponsabilidadEmpresa
                                      );

                                      row.RelativeItem(1)
                                     .PaddingVertical(3, Unit.Millimetre)
                                     .DefaultTextStyle(f => f.FontSize(10))
                                     .DefaultTextStyle(t => t.FontColor("#1f66ff"))
                                     .AlignRight()
                                     .Text("Cuit: " + empresa!.Cuit
                                     + "\n" + "IIBB: " + empresa!.Iibb
                                     + "\n" + "I. Actividades: " + empresa!.InicioActividades
                                     );
                                  });
                                  column.Item().Row(row =>
                                  {
                                      row.RelativeItem(1)
                                     .DefaultTextStyle(f => f.FontSize(10))
                                     .BorderTop(1, Unit.Millimetre).BorderColor("#858796")
                                     .PaddingVertical(3, Unit.Millimetre)
                                     .DefaultTextStyle(t => t.SemiBold())
                                     .Text("Cliente: " + cliente!.Nombre
                                     + "\n" + cliente!.Domicilio
                                     );

                                      row.RelativeItem(1)
                                      .DefaultTextStyle(f => f.FontSize(10))
                                      .BorderTop(1, Unit.Millimetre).BorderColor("#858796")
                                      .PaddingVertical(3, Unit.Millimetre)
                                      .DefaultTextStyle(t => t.SemiBold())
                                      .AlignRight()
                                      .Text("Cuit: " + cliente!.Cuit
                                      + "\n" + "RESPONSABLE " + cliente!.Responsabilidad
                                      );
                                  });
                              });
                            grid.Item(1).PaddingHorizontal(35).LineVertical(1).LineColor(Colors.Grey.Medium);
                            grid.Item(5)
                            .Column(column =>
                            {
                                column.Item().Height(35, Unit.Millimetre).Row(row =>
                                {
                                    row.RelativeItem(1).Image("Images/logo.jpg");

                                    row.RelativeItem(1).Border(1, Unit.Point)
                                   .Grid(grid =>
                                   {
                                       grid.Columns(1);
                                       grid.Item()
                                   .DefaultTextStyle(x => x.FontSize(40))
                                   .DefaultTextStyle(f => f.Bold())
                                   .AlignCenter()
                                   .PaddingVertical(1, Unit.Millimetre)
                                   .Text(text =>
                                   {
                                       text.Span("R");
                                   });
                                       grid.Item()
                                      .DefaultTextStyle(x => x.FontSize(10))
                                      .AlignCenter()
                                      .PaddingBottom(3, Unit.Millimetre)
                                      .Text(text =>
                                      {
                                          text.Span("COPIA");
                                      });
                                   });

                                    row.Spacing(10);
                                    row.RelativeItem(2)
                                    .Border(1, Unit.Point)
                                    .Padding(5, Unit.Millimetre)
                                    .DefaultTextStyle(x => x.FontSize(12))
                                    .DefaultTextStyle(f => f.Bold())
                                    .Grid(grid =>
                                    {
                                        grid.Columns(1);
                                        grid.Item()
                                         .Text(text =>
                                         {
                                             text.AlignCenter();
                                             text.Span("RECIBO");
                                         });
                                        grid.Item()
                                         .Text(text =>
                                         {
                                             text.AlignRight();
                                             text.Span("\n" + "FC: " + recibo.Id.ToString("D10"));
                                         });
                                        grid.Item()
                                        .Text(text =>
                                        {
                                            text.DefaultTextStyle(f => f.FontSize(12));
                                            text.AlignRight();
                                            text.Span("FECHA: " + recibo.Fecha.ToShortDateString());
                                        });
                                    });

                                });
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem(2)
                                    .PaddingVertical(3, Unit.Millimetre)
                                    .DefaultTextStyle(f => f.FontSize(10))
                                    .DefaultTextStyle(t => t.FontColor("#1f66ff"))
                                    .Text("Razón Social: " + empresa!.Razon
                                    + "\n" + empresa!.Fantasia
                                    + "\n" + empresa!.Domicilio
                                    + "\n" + empresa!.ResponsabilidadEmpresa
                                    );

                                    row.RelativeItem(1)
                                   .PaddingVertical(3, Unit.Millimetre)
                                   .DefaultTextStyle(f => f.FontSize(10))
                                   .DefaultTextStyle(t => t.FontColor("#1f66ff"))
                                   .AlignRight()
                                   .Text("Cuit: " + empresa!.Cuit
                                   + "\n" + "IIBB: " + empresa!.Iibb
                                   + "\n" + "I. Actividades: " + empresa!.InicioActividades
                                   );
                                });
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem(1)
                                   .DefaultTextStyle(f => f.FontSize(10))
                                   .BorderTop(1, Unit.Millimetre).BorderColor("#858796")
                                   .PaddingVertical(3, Unit.Millimetre)
                                   .DefaultTextStyle(t => t.SemiBold())
                                   .Text("Cliente: " + cliente!.Nombre
                                   + "\n" + cliente!.Domicilio
                                   );

                                    row.RelativeItem(1)
                                    .DefaultTextStyle(f => f.FontSize(10))
                                    .BorderTop(1, Unit.Millimetre).BorderColor("#858796")
                                    .PaddingVertical(3, Unit.Millimetre)
                                    .DefaultTextStyle(t => t.SemiBold())
                                    .AlignRight()
                                    .Text("Cuit: " + cliente!.Cuit
                                    + "\n" + "RESPONSABLE " + cliente!.Responsabilidad
                                    );
                                });
                            });
                        });
                        page.Content()
                       .Grid(grid =>
                       {
                           grid.Columns(11);
                           grid.Item(5)
                                .PaddingVertical(3, Unit.Millimetre)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(8);
                                        columns.RelativeColumn(3);
                                    });
                                    table.Cell().ColumnSpan(1)
                                   .Background("#9ca4df")
                                   .AlignCenter()
                                   .Text("Detalle de Pagos");
                                    table.Cell().ColumnSpan(1)
                                     .Background("#9ca4df")
                                     .AlignRight()
                                   .Text("");
                                    foreach (ReciboDetalle? c in recibo.ReciboDetalles!)
                                    {
                                        table.Cell().Padding(2).DefaultTextStyle(x => x.FontSize(10)).DefaultTextStyle(x => x.SemiBold()).AlignLeft().Text(c.Tipo + "   (" + c.Codigo + ")");
                                        table.Cell().Padding(2).DefaultTextStyle(x => x.FontSize(12)).DefaultTextStyle(x => x.SemiBold()).AlignRight().Text("$ " + c.Monto);
                                    }
                                });
                           grid.Item(1).PaddingHorizontal(35).LineVertical(1).LineColor(Colors.Grey.Medium);
                           grid.Item(5)
                                .PaddingVertical(3, Unit.Millimetre)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(8);
                                        columns.RelativeColumn(3);
                                    });
                                    table.Cell().ColumnSpan(1)
                                   .Background("#9ca4df")
                                   .AlignCenter()
                                   .Text("Detalle de Pagos");
                                    table.Cell().ColumnSpan(1)
                                     .Background("#9ca4df")
                                     .AlignRight()
                                   .Text("");
                                    foreach (ReciboDetalle? c in recibo.ReciboDetalles!)
                                    {
                                        table.Cell().Padding(2).DefaultTextStyle(x => x.FontSize(10)).DefaultTextStyle(x => x.SemiBold()).AlignLeft().Text(c.Tipo + "   (" + c.Codigo + ")");
                                        table.Cell().Padding(2).DefaultTextStyle(x => x.FontSize(12)).DefaultTextStyle(x => x.SemiBold()).AlignRight().Text("$ " + c.Monto);
                                    }
                                });
                       });
                        page.Footer()
                       .Grid(grid =>
                       {
                           grid.Columns(11);
                           grid.Item(5)
                               .BorderTop(1, Unit.Point)
                               .Column(c =>
                               {
                                   c.Item().Row(r =>
                                   {
                                       r.RelativeItem(7)
                                      .DefaultTextStyle(t => t.FontSize(10))
                                      .DefaultTextStyle(x => x.Bold())
                                       .Text(text =>
                                       {
                                           text.AlignRight();
                                           text.Line("Recibimos la suma de " + letras);
                                           text.AlignLeft();
                                           text.Span("\n" + "Ud. fue atendido por " + recibo.Operador);

                                       });
                                       r.RelativeItem(5)
                                      .DefaultTextStyle(t => t.FontSize(12))
                                      .DefaultTextStyle(x => x.Bold())
                                      .AlignRight()
                                      .Text(text =>
                                      {
                                          text.Line("TOTAL $: " + total.ToString());
                                      });
                                   });
                                   c.Item()
                                   .PaddingTop(5)
                                   .BorderTop(1, Unit.Point)
                                   .Text(text =>
                                   {
                                       text.DefaultTextStyle(f => f.FontSize(8));
                                       text.AlignLeft();
                                       text.Span("2022 © Desarrollado por Aramis Sistemas");
                                   });
                               });
                           grid.Item(1).PaddingHorizontal(35).LineVertical(1).LineColor(Colors.Grey.Medium);
                           grid.Item(5)
                               .BorderTop(1, Unit.Point)
                               .Column(c =>
                               {
                                   c.Item().Row(r =>
                                   {
                                       r.RelativeItem(7)
                                      .DefaultTextStyle(t => t.FontSize(10))
                                      .DefaultTextStyle(x => x.Bold())
                                       .Text(text =>
                                       {
                                           text.AlignRight();
                                           text.Line("Recibimos la suma de " + letras);
                                           text.AlignLeft();
                                           text.Span("\n" + "Ud. fue atendido por " + recibo.Operador);

                                       });
                                       r.RelativeItem(5)
                                      .DefaultTextStyle(t => t.FontSize(12))
                                      .DefaultTextStyle(x => x.Bold())
                                      .AlignRight()
                                      .Text(text =>
                                      {
                                          text.Line("TOTAL $: " + total.ToString());
                                      });
                                   });
                                   c.Item()
                                   .PaddingTop(5)
                                   .BorderTop(1, Unit.Point)
                                   .Text(text =>
                                   {
                                       text.DefaultTextStyle(f => f.FontSize(8));
                                       text.AlignLeft();
                                       text.Span("2022 © Desarrollado por Aramis Sistemas");
                                   });
                               });
                       });
                    });
                })
                .GeneratePdf(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}

