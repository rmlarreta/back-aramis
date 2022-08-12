using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QRCoder; 
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using System.Drawing;
using System.Text.Json;

namespace backaramis.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IStoreProcedure _storeProcedure;
        private readonly aramisContext _context;

        public DocumentService(IStoreProcedure storeProcedure, aramisContext context)
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
                    ds = _storeProcedure.SpWhithDataSetPure("DocumentoGet", Params);
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
                        Pos = row["Pos"].ToString(),
                        Numero = row["Numero"].ToString(),
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
                        DomicilioCliente = row["DomicilioCliente"].ToString(),
                        ResponsabilidadCliente = row["ResponsabilidadCliente"].ToString(),
                        EnLetras = row["EnLetras"].ToString(),
                        NumeroInt = (int)row["NumeroInt"],
                        PosInt = (int)row["PosInt"],
                        CodAut=(int)row["CodAut"],
                        Vence=(DateTime)row["Vence"]
                    });
                }

                foreach (DataRow rowE in dtEmpresa.Rows)
                {
                    documents.First().Razon = rowE["Razon"].ToString();
                    documents.First().CuitEmpresa = rowE["CuitEmpresa"].ToString();
                    documents.First().Fantasia = rowE["Fantasia"].ToString();
                    documents.First().IIBB = rowE["IIBB"].ToString();
                    documents.First().DomicilioEmpresa = rowE["DomicilioEmpresa"].ToString();
                    documents.First().InicioActividades = rowE["InicioActividades"].ToString();
                    documents.First().ResponsabilidadEmpresa = rowE["ResponsabilidadEmpresa"].ToString();
                }

                foreach (DataRow rowQ in dtDetalles.Rows)
                {
                    detalles.Add(new DocumentsDetallDto()
                    {
                        Cantidad = (decimal)rowQ["Cantidad"],
                        Detalle = rowQ["Detalle"].ToString(),
                        Documento = (long)rowQ["Documento"],
                        Id = (long)rowQ["Id"],
                        Internos = (decimal)rowQ["Internos"],
                        Iva = (decimal)rowQ["Iva"],
                        Unitario = (decimal)rowQ["Unitario"],
                        Codigo = rowQ["Codigo"].ToString(),
                        Producto = (long)rowQ["Producto"],
                        Rubro = rowQ["Rubro"].ToString(),
                        SubTotal = (decimal)rowQ["SubTotal"]
                    });
                }
                Documents data = new();
                data.DocumentsDto = documents;
                data.DocumentsDetallDto = detalles;
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public void InsertDocument(string Operador)
        {
            try
            {
                List<SqlParameter> Params = new();
                Params.Add(new SqlParameter("@operador", Operador));
                _storeProcedure.ExecuteNonQuery("DocumentInsert", Params);
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
                Documento? document = _context.Documentos.FirstOrDefault(x => x.Id == Id);
                if (document == null)
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
        public FileStreamResult ReporteRemito(int id)
        {
            var remito = GetDocuments(id);
            if (remito == null)
            {
                return null;
            }
            var stream = new MemoryStream();
            var QrJsonModel = new QrJson
            {
                CodAut = 0,// remito.DocumentsDto.First().CodAuto
                Ctz = 1,
                Cuit = remito.DocumentsDto.First().CuitEmpresa.Replace("-", ""),
                Fecha = remito.DocumentsDto.First().Fecha.ToString("yyyy-MM-dd"),
                Importe = remito.DocumentsDto.First().Total,
                Moneda = "PES",
                NroCmp = remito.DocumentsDto.First().NumeroInt,
                NroDocRec = remito.DocumentsDto.First().Cuit,
                PtoVenta = remito.DocumentsDto.First().PosInt,
                TipoCmp = 88,
                TipoCodAut = "O",
                TipoDocRec = remito.DocumentsDto.First().Cuit == 0 ? 99 : 86,
                Ver=1
            };
            var QrString = JsonSerializer.Serialize(QrJsonModel);            
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(QrString);
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("Remito"+ System.Convert.ToBase64String(plainTextBytes), QRCodeGenerator.ECCLevel.Q);
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
                                        text.Span(remito.DocumentsDto.First().Letra);
                                    });
                                        grid.Item()
                                       .DefaultTextStyle(x => x.FontSize(10))
                                       .AlignCenter()
                                       .PaddingBottom(3, Unit.Millimetre)
                                       .Text(text =>
                                       {
                                           text.Span(remito.DocumentsDto.First().Tipo);
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
                                              text.Span(remito.DocumentsDto.First().Tipo);
                                          });
                                         grid.Item()
                                          .Text(text =>
                                          {
                                              text.AlignRight();
                                              text.Span("\n FC: " + remito.DocumentsDto.First().Pos + " - " + remito.DocumentsDto.First().Numero);
                                          });
                                         grid.Item()
                                         .Text(text =>
                                         {
                                             text.DefaultTextStyle(f => f.NormalWeight());
                                             text.AlignRight();
                                             text.Span("FECHA: " + remito.DocumentsDto.First().Fecha.ToShortDateString());
                                         });
                                     });

                                 });
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem(1)
                                    .PaddingVertical(3, Unit.Millimetre)
                                    .DefaultTextStyle(f => f.FontSize(10))
                                    .DefaultTextStyle(t => t.FontColor("#1f66ff"))
                                    .Text("Razón Social: " + remito.DocumentsDto.First().Razon
                                    + "\n" + remito.DocumentsDto.First().Fantasia
                                    + "\n" + remito.DocumentsDto.First().DomicilioEmpresa
                                    + "\n" + remito.DocumentsDto.First().ResponsabilidadEmpresa
                                    );

                                    row.RelativeItem(1)
                                   .PaddingVertical(3, Unit.Millimetre)
                                   .DefaultTextStyle(f => f.FontSize(10))
                                   .DefaultTextStyle(t => t.FontColor("#1f66ff"))
                                   .Text("Cuit: " + remito.DocumentsDto.First().CuitEmpresa
                                   + "\n" + "IIBB: " + remito.DocumentsDto.First().IIBB
                                   + "\n" + "I. Actividades: " + remito.DocumentsDto.First().InicioActividades
                                   );
                                });

                                column.Item().Row(row =>
                                {
                                    row.RelativeItem(1)
                                   .DefaultTextStyle(f => f.FontSize(10))
                                   .BorderTop(1, Unit.Millimetre).BorderColor("#858796")
                                   .PaddingVertical(2, Unit.Millimetre)
                                   .DefaultTextStyle(t => t.SemiBold())
                                   .Text("Cliente: " + remito.DocumentsDto.First().Nombre
                                   + "\n" + remito.DocumentsDto.First().DomicilioCliente
                                   );

                                    row.RelativeItem(1)
                                    .DefaultTextStyle(f => f.FontSize(10))
                                    .BorderTop(1, Unit.Millimetre).BorderColor("#858796")
                                    .PaddingVertical(2, Unit.Millimetre)
                                    .DefaultTextStyle(t => t.SemiBold())
                                    .Text("Cuit: " + remito.DocumentsDto.First().Cuit
                                    + "\n" + "RESPONSABLE " + remito.DocumentsDto.First().ResponsabilidadCliente
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

                                    foreach (var c in remito.DocumentsDetallDto)
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
                                      .DefaultTextStyle(t => t.FontSize(10))
                                      .DefaultTextStyle(x => x.Bold())
                                      .Text("Neto Gravado: $ " + Math.Round(remito.DocumentsDto.First().Neto, 2)
                                      + "\n" + "Excento: $ " + Math.Round(remito.DocumentsDto.First().Excento, 2)
                                      + "\n" + "IVA 10.5%: $ " + Math.Round(remito.DocumentsDto.First().Iva10, 2)
                                      + "\n" + "IVA 21.0%: $ " + Math.Round(remito.DocumentsDto.First().Iva21, 2)
                                      + "\n" + "Imp.Internos: $ " + Math.Round(remito.DocumentsDto.First().Internos, 2));
                                       r.RelativeItem()
                                      .DefaultTextStyle(t => t.FontSize(8))
                                      .DefaultTextStyle(x => x.Bold())
                                          .Text(text =>
                                          {
                                              text.Span("\n" + "\n" + "\n" + "Ud. fue atendido por " + remito.DocumentsDto.First().Operador);
                                          });
                                       r.RelativeItem(2)
                                      .DefaultTextStyle(t => t.FontSize(12))
                                      .DefaultTextStyle(x => x.Bold())
                                      .AlignRight()
                                      .Text(text =>
                                        {
                                            text.Line("TOTAL $: " + Math.Round(remito.DocumentsDto.First().Total, 2));
                                            text.Line(remito.DocumentsDto.First().EnLetras).FontSize(8);
                                        });
                                   });
                                   c.Item()
                                   .PaddingTop(5)
                                   .BorderTop(1, Unit.Point).Grid(grid =>
                                       {
                                           grid.VerticalSpacing(2); 
                                           grid.HorizontalSpacing(2);
                                           grid.AlignLeft();
                                           grid.Columns(12); // 12 by default

                                           //grid.Item(2).Height(100).Image(qrCodeAsBitmapByteArr, ImageScaling.FitArea);
                                           //grid.Item(5).Height(80).Image("Images/Afip.jpg", ImageScaling.FitArea); 
                                           //grid.Item(5).Height(60).AlignRight().Text(t =>
                                           //{ 
                                           //    t.DefaultTextStyle(t => t.FontSize(12));
                                           //    t.DefaultTextStyle(x => x.Bold()); 
                                           //    t.Line("CAE Nº: " + remito.DocumentsDto.First().CodAut.ToString());
                                           //    t.Line("Fecha de Vto de CAE: " + remito.DocumentsDto.First().Vence.ToShortDateString());
                                           //}
                                           //);
                                              grid.Item(6).Height(20).AlignLeft().DefaultTextStyle(f=>f.FontSize(10)).Text("2022 © Desarrollado por Aramis Sistemas"); 
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
    }
}
