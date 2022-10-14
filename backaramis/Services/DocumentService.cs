using AfipServiceReference;
using AfipWsfeClient;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using System.Text.Json;

namespace backaramis.Services
{
    public class DocumentService : IDocumentService, IFiscalService
    {
        private readonly IStoreProcedure _storeProcedure;
        private readonly AramisContext _context;

        public DocumentService(IStoreProcedure storeProcedure, AramisContext context)
        {
            _storeProcedure = storeProcedure;
            _context = context;
        }
        public Documents GetDocuments(long? Id = null, int? tipo = null, int? estado = null)
        {

            try
            {
                DataSet ds = new();
                if (Id == null && tipo == null && estado == null)
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
                    if (estado != null)
                    {
                        Params.Add(new SqlParameter("@estado", estado));
                    }
                    ds = _storeProcedure.SpWhithDataSetPure(tipo == 3 ? "RemitosGet" : "DocumentoGet", Params);
                }
                List<DocumentsDto> documents = new();
                List<DocumentsDetallDto> detalles = new();
                DataTable dtDocuments = new();
                DataTable dtDetalles = new();
                DataTable dtEmpresa = new();
                dtDocuments = ds.Tables[0];
                dtDetalles = ds.Tables[1];
                dtEmpresa = ds.Tables[2];

                foreach (DataRow row in dtDocuments.Rows)
                {
                    documents.Add(new DocumentsDto()
                    {
                        //header
                        Id = (long)row["Id"],
                        CodTipo = (int)row["CodTipo"],
                        Letra = row["Letra"].ToString(),
                        Tipo = row["Tipo"].ToString(),
                        Pos = row["Pos"].ToString()!,
                        Numero = row["Numero"].ToString()!,
                        Fecha = (DateTime)row["Fecha"],
                        Cuit = (long)row["Cuit"],
                        Nombre = row["Nombre"].ToString(),
                        Cliente = (long)row["Cliente"],
                        Observaciones = row["Observaciones"].ToString(),
                        Operador = row["Operador"].ToString(),
                        Total = (decimal)row["Total"],
                        Limite = (decimal)row["Limite"],
                        Neto = (decimal)row["Neto"],
                        Internos = (decimal)row["Internos"],
                        Excento = (decimal)row["Excento"],
                        Iva10 = (decimal)row["Iva10"],
                        Iva21 = (decimal)row["Iva21"],
                        DomicilioCliente = row["DomicilioCliente"].ToString()!,
                        ResponsabilidadCliente = row["ResponsabilidadCliente"].ToString()!,
                        EnLetras = row["EnLetras"].ToString(),
                        NumeroInt = (int)row["NumeroInt"],
                        PosInt = (int)row["PosInt"],
                        CodAut = row["CodAut"].ToString(),
                        Vence = (DateTime)row["Vence"]
                    });
                }

                foreach (DataRow rowE in dtEmpresa.Rows)
                {
                    documents.First().Razon = rowE["Razon"].ToString()!;
                    documents.First().CuitEmpresa = rowE["CuitEmpresa"].ToString()!;
                    documents.First().Fantasia = rowE["Fantasia"].ToString()!;
                    documents.First().IIBB = rowE["IIBB"].ToString()!;
                    documents.First().DomicilioEmpresa = rowE["DomicilioEmpresa"].ToString()!;
                    documents.First().InicioActividades = rowE["InicioActividades"].ToString()!;
                    documents.First().ResponsabilidadEmpresa = rowE["ResponsabilidadEmpresa"].ToString()!;
                }

                foreach (DataRow rowQ in dtDetalles.Rows)
                {
                    detalles.Add(new DocumentsDetallDto()
                    {
                        Cantidad = (decimal)rowQ["Cantidad"],
                        Detalle = rowQ["Detalle"].ToString()!,
                        Documento = (long)rowQ["Documento"],
                        Id = (long)rowQ["Id"],
                        Internos = (decimal)rowQ["Internos"],
                        Iva = (decimal)rowQ["Iva"],
                        Unitario = (decimal)rowQ["Unitario"],
                        Codigo = rowQ["Codigo"].ToString()!,
                        Producto = (long)rowQ["Producto"],
                        Rubro = rowQ["Rubro"].ToString()!,
                        SubTotal = (decimal)rowQ["SubTotal"]
                    });
                }
                Documents data = new();
                data.DocumentsDto = documents;
                data.DocumentsDetallDto = detalles;
                if (data != null)
                {
                    return data;
                }

                return null!;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Sequence contains no elements")
                {
                    return null!;
                }

                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
        public async Task<Documento> InsertDocument(string Operador)
        {
            try
            {
                SystemOption? systemOption = await _context.SystemOptions.FirstOrDefaultAsync();
                if (systemOption == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }
                systemOption.P += 1; //numero de recibo
                _context.SystemOptions.Attach(systemOption);

                Cliente? mostrador = await _context.Clientes.FirstAsync(x => x.Cuit == 0);

                Documento documento = new()
                {
                    Tipo = 1,
                    Pos = 0,
                    Numero = systemOption.P,
                    Cliente = mostrador.Id,
                    Fecha = DateTime.Now,
                    CodAut = String.Empty,
                    Vence = DateTime.Now,
                    Razon = mostrador.Nombre ?? String.Empty,
                    Observaciones = String.Empty,
                    Operador = Operador,
                    Creado = DateTime.Now
                };

                await _context.Documentos.AddAsync(documento);
                await _context.SaveChangesAsync();

                return documento;
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
                Documento? document = _context.Documentos.FirstOrDefault(x => x.Id == Id);
                if (document == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }
                if (_context.Clientes.FirstOrDefaultAsync(x => x.Id == document.Cliente).Result!.Cuit == 0)
                {
                    throw new Exception("EL MOSTRADOR NO PUEDE TENER ORDENES DE SERVICIO");
                }
                DocumentoTipo? tipo = _context.DocumentoTipos.FirstOrDefault(x => x.Letra == "O");
                SystemOption? numero = _context.SystemOptions.FirstOrDefault();
                if (numero == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }
                numero.O += 1;
                if (tipo == null)
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
                Documento? document = _context.Documentos.FirstOrDefault(x => x.Id == Id);
                Cliente? persona = _context.Clientes.First(x => x.Id == cliente);
                if (document == null)
                {
                    throw new Exception("Verifique, las tablas no están listas");
                }
                if (document.Tipo == 2 && persona.Cuit == 0)
                {
                    throw new Exception("Las ódenes de servicio, no se pueden generar al MOSTRADOR");
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
        public FileStreamResult Report(int id)
        {
            Documents? documento = GetDocuments(id);
            if (documento == null)
            {
                return null!;
            }
            MemoryStream? stream = new();
            QrJson? QrJsonModel = new()
            {
                CodAut = documento.DocumentsDto!.First().CodTipo > 3 ? documento.DocumentsDto!.First().CodAut! : "0",
                Ctz = 1,
                Cuit = documento.DocumentsDto!.First().CuitEmpresa!.Replace("-", ""),
                Fecha = documento.DocumentsDto!.First().Fecha.ToString("yyyy-MM-dd"),
                Importe = documento.DocumentsDto!.First().Total,
                Moneda = "PES",
                NroCmp = documento.DocumentsDto!.First().NumeroInt,
                NroDocRec = documento.DocumentsDto!.First().Cuit,
                PtoVenta = documento.DocumentsDto!.First().PosInt,
                TipoCmp = documento.DocumentsDto!.First().CodTipo == 3 ? 88 : documento.DocumentsDto!.First().CodTipo == 4 ? 1 : documento.DocumentsDto!.First().CodTipo == 5 ? 6 : documento.DocumentsDto!.First().CodTipo == 6 ? 3 : documento.DocumentsDto!.First().CodTipo == 7 ? 8 : 0,
                TipoCodAut = "E",
                TipoDocRec = documento.DocumentsDto!.First().Cuit == 0 ? 99 : 86,
                Ver = 1
            };
            string? QrString = JsonSerializer.Serialize(QrJsonModel);
            byte[]? plainTextBytes = System.Text.Encoding.UTF8.GetBytes(QrString);
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://www.afip.gob.ar/fe/qr/?p=" + Convert.ToBase64String(plainTextBytes), QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

            Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            //seteamos la página
                            page.Size(PageSizes.A4);
                            page.Margin(5, Unit.Millimetre);
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(20));
                            page.DefaultTextStyle(x => x.FontFamily("Arial"));

                            //seteamos el encabezado 
                            page.Header()
                            .Column(column =>
                            {
                                column.Item().Height(35, Unit.Millimetre).Row(row =>
                                 {
                                     row.RelativeItem(1).Image("Images/logo.jpg");

                                     row.RelativeItem(1).DefaultTextStyle(x => x.FontSize(12))
                                     .DefaultTextStyle(f => f.Bold())
                                     .AlignCenter()
                                     .Text(text =>
                                              {
                                                  text.Span("Página ");
                                                  text.CurrentPageNumber();
                                                  text.Span(" de ");
                                                  text.TotalPages();
                                              });

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
                                        text.Span(documento.DocumentsDto!.First().Letra);
                                    });
                                        grid.Item()
                                       .DefaultTextStyle(x => x.FontSize(10))
                                       .AlignCenter()
                                       .PaddingBottom(3, Unit.Millimetre)
                                       .Text(text =>
                                       {
                                           text.Span(documento.DocumentsDto!.First().Tipo);
                                       });
                                    });

                                     row.Spacing(20);
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
                                              text.Span(documento.DocumentsDto!.First().Tipo);
                                          });
                                         grid.Item()
                                          .Text(text =>
                                          {
                                              text.AlignRight();
                                              text.Span("\n FC: " + documento.DocumentsDto!.First().Pos + " - " + documento.DocumentsDto!.First().Numero);
                                          });
                                         grid.Item()
                                         .Text(text =>
                                         {
                                             text.DefaultTextStyle(f => f.NormalWeight());
                                             text.AlignRight();
                                             text.Span("FECHA: " + documento.DocumentsDto!.First().Fecha.ToShortDateString());
                                         });
                                     });

                                 });
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem(1)
                                    .PaddingVertical(3, Unit.Millimetre)
                                    .DefaultTextStyle(f => f.FontSize(10))
                                    .DefaultTextStyle(t => t.FontColor("#1f66ff"))
                                    .Text("Razón Social: " + documento.DocumentsDto!.First().Razon
                                    + "\n" + documento.DocumentsDto!.First().Fantasia
                                    + "\n" + documento.DocumentsDto!.First().DomicilioEmpresa
                                    + "\n" + documento.DocumentsDto!.First().ResponsabilidadEmpresa
                                    );

                                    row.RelativeItem(1)
                                   .PaddingVertical(3, Unit.Millimetre)
                                   .DefaultTextStyle(f => f.FontSize(10))
                                   .DefaultTextStyle(t => t.FontColor("#1f66ff"))
                                   .Text("Cuit: " + documento.DocumentsDto!.First().CuitEmpresa
                                   + "\n" + "IIBB: " + documento.DocumentsDto!.First().IIBB
                                   + "\n" + "I. Actividades: " + documento.DocumentsDto!.First().InicioActividades
                                   );
                                });

                                column.Item().Row(row =>
                                {
                                    row.RelativeItem(1)
                                   .DefaultTextStyle(f => f.FontSize(10))
                                   .BorderTop(1, Unit.Millimetre).BorderColor("#858796")
                                   .PaddingVertical(2, Unit.Millimetre)
                                   .DefaultTextStyle(t => t.SemiBold())
                                   .Text("Cliente: " + documento.DocumentsDto!.First().Nombre
                                   + "\n" + documento.DocumentsDto!.First().DomicilioCliente
                                   );

                                    row.RelativeItem(1)
                                    .DefaultTextStyle(f => f.FontSize(10))
                                    .BorderTop(1, Unit.Millimetre).BorderColor("#858796")
                                    .PaddingVertical(2, Unit.Millimetre)
                                    .DefaultTextStyle(t => t.SemiBold())
                                    .Text("Cuit: " + documento.DocumentsDto!.First().Cuit
                                    + "\n" + "RESPONSABLE " + documento.DocumentsDto!.First().ResponsabilidadCliente
                                    );
                                });
                            });

                            //setemaos los detalles
                            page.Content()
                                .PaddingVertical(1, Unit.Millimetre)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(10);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(2);
                                    });
                                    table.Cell().ColumnSpan(1)
                                   .Background("#9ca4df")
                                   .Text("Código");
                                    table.Cell().ColumnSpan(1)
                                     .Background("#9ca4df")
                                   .Text("Detalle");
                                    table.Cell().ColumnSpan(1)
                                     .Background("#9ca4df")
                                   .Text("Cantidad");
                                    table.Cell().ColumnSpan(1)
                                     .Background("#9ca4df")
                                   .Text("Unitario");
                                    table.Cell().ColumnSpan(1)
                                     .Background("#9ca4df")
                                   .Text("Iva");
                                    table.Cell().ColumnSpan(1)
                                     .Background("#9ca4df")
                                   .Text("Sub Total");

                                    foreach (DocumentsDetallDto? c in documento.DocumentsDetallDto!)
                                    {
                                        table.Cell().Padding(0).DefaultTextStyle(x => x.FontSize(8)).DefaultTextStyle(x => x.NormalWeight()).AlignCenter().Text(c.Codigo);
                                        table.Cell().Padding(0).DefaultTextStyle(x => x.FontSize(8)).DefaultTextStyle(x => x.NormalWeight()).AlignLeft().Text(c.Detalle);
                                        table.Cell().Padding(0).DefaultTextStyle(x => x.FontSize(8)).DefaultTextStyle(x => x.NormalWeight()).AlignCenter().Text(c.Cantidad);
                                        table.Cell().Padding(0).DefaultTextStyle(x => x.FontSize(8)).DefaultTextStyle(x => x.NormalWeight()).AlignCenter().Text("$ " + c.Unitario);
                                        table.Cell().Padding(0).DefaultTextStyle(x => x.FontSize(8)).DefaultTextStyle(x => x.NormalWeight()).AlignCenter().Text("% " + c.Iva);
                                        table.Cell().Padding(0).DefaultTextStyle(x => x.FontSize(8)).DefaultTextStyle(x => x.NormalWeight()).AlignRight().Text("$ " + Math.Round(c.SubTotal, 2));
                                    }
                                });
                            //seteamos los footer                            
                            page.Footer()
                               .BorderTop(1, Unit.Point)
                               .Column(c =>
                               {
                                   c.Item().Row(r =>
                                   {
                                       r.RelativeItem()
                                       .Text(documento.DocumentsDto!.First().Observaciones);
                                   });
                                   c.Item().BorderTop(1, Unit.Point);
                                   c.Item().Row(r =>
                                   {
                                       r.RelativeItem()
                                      .DefaultTextStyle(t => t.FontSize(10))
                                      .DefaultTextStyle(x => x.Bold())
                                      .Text("Neto Gravado: $ " + Math.Round(documento.DocumentsDto!.First().Neto, 2)
                                      + "\n" + "Excento: $ " + Math.Round(documento.DocumentsDto!.First().Excento, 2)
                                      + "\n" + "IVA 10.5%: $ " + Math.Round(documento.DocumentsDto!.First().Iva10, 2)
                                      + "\n" + "IVA 21.0%: $ " + Math.Round(documento.DocumentsDto!.First().Iva21, 2)
                                      + "\n" + "Imp.Internos: $ " + Math.Round(documento.DocumentsDto!.First().Internos, 2));
                                       r.RelativeItem()
                                      .DefaultTextStyle(t => t.FontSize(8))
                                      .DefaultTextStyle(x => x.Bold())
                                          .Text(text =>
                                          {
                                              text.Span("\n" + "\n" + "\n" + "Ud. fue atendido por " + documento.DocumentsDto!.First().Operador);
                                          });
                                       r.RelativeItem(2)
                                      .DefaultTextStyle(t => t.FontSize(12))
                                      .DefaultTextStyle(x => x.Bold())
                                      .AlignRight()
                                      .Text(text =>
                                        {
                                            text.Line("TOTAL $: " + Math.Round(documento.DocumentsDto!.First().Total, 2));
                                            text.Line(documento.DocumentsDto!.First().EnLetras).FontSize(8);
                                        });
                                   });
                                   c.Item()
                                   .PaddingTop(5)
                                   .BorderTop(1, Unit.Point)
                                   .Grid(grid =>
                                       {
                                           grid.VerticalSpacing(2);
                                           grid.HorizontalSpacing(2);
                                           grid.AlignLeft();
                                           grid.Columns(12); // 12 by default

                                           if (documento.DocumentsDto!.First().CodTipo > 3)
                                           {
                                               grid.Item(2).Height(100).Image(qrCodeAsBitmapByteArr, ImageScaling.FitArea);
                                               grid.Item(5).Height(80).Image("Images/Afip.jpg", ImageScaling.FitArea);
                                               grid.Item(5).Height(60).AlignRight().Text(t =>
                                               {
                                                   t.DefaultTextStyle(t => t.FontSize(12));
                                                   t.DefaultTextStyle(x => x.Bold());
                                                   t.Line("CAE Nº: " + documento.DocumentsDto!.First().CodAut!);
                                                   t.Line("Fecha de Vto de CAE: " + documento.DocumentsDto!.First().Vence.ToShortDateString());
                                               });
                                           }

                                           grid.Item(6).Height(20).AlignLeft().DefaultTextStyle(f => f.FontSize(10)).Text("2022 © Desarrollado por Aramis Sistemas");
                                           grid.Item(6).Height(20).AlignCenter().DefaultTextStyle(f => f.FontSize(10)).Text(t =>
                                           {
                                               t.Span("Página ");
                                               t.CurrentPageNumber();
                                               t.Span(" de ");
                                               t.TotalPages();
                                           });
                                       });
                               });
                        });
                    })
                   .GeneratePdf(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(stream, "application/pdf");
        }
        public async Task<FECAESolicitarResponse> GetCae(DocumentoFiscal documento)
        {
            try
            {

                #region Get Login Ticket
                //Get Login Ticket
                LoginCmsClient? loginClient = new() { IsProdEnvironment = _context.SystemOptions.First().Produccion };
                WsaaTicket? ticket = await loginClient.LoginCmsAsync("wsfe",
                                                             "Certificados/certificado.p12",
                                                             "1234",
                                                             true);
                #endregion

                #region wsfeClient

                WsfeClient? wsfeClient = new()
                {
                    IsProdEnvironment = _context.SystemOptions.First().Produccion,
                    Cuit = long.Parse(_context.SystemOptions.First().Cuit.Replace("-", "")),
                    Sign = ticket.Sign,
                    Token = ticket.Token
                };
                int compNumber = wsfeClient.FECompUltimoAutorizadoAsync(_context.SystemOptions.First().PtoVenta, documento.TipoComprobante)
                    .Result.Body.FECompUltimoAutorizadoResult.CbteNro + 1;

                #endregion

                #region Build WSFE Request
                //Build WSFE FECAERequest          
                FECAERequest? feCaeReq = new()
                {
                    FeCabReq = new FECAECabRequest { CantReg = 1, CbteTipo = documento.TipoComprobante, PtoVta = _context.SystemOptions.First().PtoVenta },
                    FeDetReq = new List<FECAEDetRequest> { new FECAEDetRequest { CbteDesde = compNumber, CbteHasta = compNumber, CbteFch = DateTime.Now.ToString("yyyyMMdd"), Concepto = 3, DocNro = documento.DocumentoCliente, DocTipo = documento.TipoDocumento, FchVtoPago = DateTime.Now.ToString("yyyyMMdd"), ImpNeto = (double)documento.Neto, ImpTotal = (double)documento.TotalComprobante, ImpIVA = (double)documento.IvaTotal, ImpOpEx = (double)documento.Exento, ImpTrib = (double)documento.Internos, FchServDesde = DateTime.Now.ToString("yyyyMMdd"), FchServHasta = DateTime.Now.ToString("yyyyMMdd"), Tributos = documento.Tributo, MonCotiz = 1, MonId = "PES", Iva = documento.Alicuotas } }
                };

                #endregion

                //Call WSFE FECAESolicitar
                FECAESolicitarResponse? compResult = await wsfeClient.FECAESolicitarAsync(feCaeReq);
                return compResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }
        public async Task<long> FacturaRemito(long id)
        {
            Documento? documento = _context.Documentos
                           .Where(d => d.Id == id)
                           .Include(dd => dd.DocumentoDetalles)
                           .FirstOrDefault();

            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@id", documento!.Id)
            };
            DataSet? totales = _storeProcedure.SpWhithDataSetPure("DocumentoGet", parameters);


            DocumentoFiscal documentoFiscal = new();
            documentoFiscal.TipoComprobante = _context.Clientes.FirstAsync(x => x.Id == documento!.Cliente).Result.Responsabilidad == "INSCRIPTO" || _context.Clientes.FirstAsync(x => x.Id == documento!.Cliente).Result.Responsabilidad == "MONOTRIBUTO" ? 1 : 6;
            documentoFiscal.TipoDocumento = _context.Clientes.FirstAsync(x => x.Id == documento!.Cliente).Result.Cuit == 0 ? 99 : 80;
            documentoFiscal.DocumentoCliente = _context.Clientes.FirstAsync(x => x.Id == documento!.Cliente).Result.Cuit;
            documentoFiscal.NoGravado = 0;
            foreach (DataRow row in totales.Tables[0].Rows)
            {
                documentoFiscal.Exento += (decimal)row["Excento"];
                documentoFiscal.Neto += (decimal)row["Neto"];
                documentoFiscal.Internos += (decimal)row["Internos"];
                documentoFiscal.Neto10 += (decimal)row["Neto10"];
                documentoFiscal.Iva10 += (decimal)row["Iva10"];
                documentoFiscal.Neto21 += (decimal)row["Neto21"];
                documentoFiscal.Iva21 += (decimal)row["Iva21"];
                documentoFiscal.IvaTotal = documentoFiscal.Iva10 + documentoFiscal.Iva21;
                documentoFiscal.TotalComprobante = (decimal)row["Total"];
            };

            List<AlicIva> alicIvas = new();
            if (documentoFiscal.Exento > 0)
            {
                alicIvas.Add(new() { Id = 3, BaseImp = (double)documentoFiscal.Exento, Importe = 0 });
            }

            if (documentoFiscal.Iva10 > 0)
            {
                alicIvas.Add(new() { Id = 4, BaseImp = (double)documentoFiscal.Neto10, Importe = (double)documentoFiscal.Iva10 });
            }

            if (documentoFiscal.Iva21 > 0)
            {
                alicIvas.Add(new() { Id = 5, BaseImp = (double)documentoFiscal.Neto21, Importe = (double)documentoFiscal.Iva21 });
            }

            documentoFiscal.Alicuotas = alicIvas;
            if (documentoFiscal.Internos > 0)
            {
                List<Tributo> tributo = new();
                tributo.Add(new() { Id = 4, BaseImp = (double)documentoFiscal.TotalComprobante, Importe = (double)documentoFiscal.Internos });
                documentoFiscal.Tributo = tributo;
            }

            FECAESolicitarResponse? cae = await GetCae(documentoFiscal);
            if (cae != null)
            {
                if (cae.Body.FECAESolicitarResult.FeCabResp.Resultado == "A")
                {
                    //var documentoDetalles = from d in _context.DocumentoDetalles
                    //                        where d.Documento == documento.Id
                    //                        select d;
                    if (documento.DocumentoDetalles.Any())
                    {
                        foreach (DocumentoDetalle? det in documento.DocumentoDetalles)
                        {
                            det.Facturado = det.Cantidad;
                        }
                    }

                    _context.Update(documento);

                    //Creamos el nuevo documento para la factura

                    Documento factura = new();
                    List<DocumentoDetalle> detallesFiscales = new();
                    foreach (DocumentoDetalle? d in documento.DocumentoDetalles)
                    {
                        detallesFiscales.Add(new DocumentoDetalle
                        {
                            Facturado = d.Cantidad,
                            Codigo = d.Codigo,
                            Detalle = d.Detalle,
                            Cantidad = d.Cantidad,
                            Internos = d.Internos,
                            Iva = d.Iva,
                            Producto = d.Producto,
                            Rubro = d.Rubro,
                            Unitario = d.Unitario
                        });
                    }

                    factura.Tipo = documentoFiscal.TipoComprobante == 6 ? 5 : 4;
                    factura.Pos = cae.Body.FECAESolicitarResult.FeCabResp.PtoVta;
                    factura.Numero = (int)cae.Body.FECAESolicitarResult.FeDetResp.First().CbteDesde;
                    factura.Cliente = documento.Cliente;
                    factura.Fecha = DateTime.Now;
                    factura.Vence = DateTime.Now;
                    factura.Razon = documento.Razon;
                    factura.Observaciones = "Remito " + documento.Numero.ToString();
                    factura.Operador = documento.Operador;
                    factura.Creado = DateTime.Now;
                    factura.Estado = documento.Estado;
                    factura.Pago = documento.Pago;
                    factura.CodAut = cae.Body.FECAESolicitarResult.FeDetResp.First().CAE;
                    factura.DocumentoDetalles = detallesFiscales;
                    Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Documento>? data = _context.Documentos.Add(factura);
                }
            }
            else
            {
                documento.Observaciones = "Pendiente de Facturación";
                _context.Update(documento);
            }
            int result = await _context.SaveChangesAsync();
            return result;

        }
        public List<long> GetDocumentsByRecibo(int recibo)
        {
            IQueryable<long>? documentos = from listado in _context.DocumentoRecibos
                                           where listado.Recibo == recibo
                                           select listado.Documento;

            return documentos.ToList();
        }
    }
}
