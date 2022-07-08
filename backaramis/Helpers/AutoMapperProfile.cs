using AutoMapper;
using backaramis.Models;
using backaramis.Modelsdtos.Clientes;
using backaramis.Modelsdtos.Commons;
using backaramis.Modelsdtos.Documents;
using backaramis.Modelsdtos.Recibos;
using backaramis.Modelsdtos.Stock;
using backaramis.Modelsdtos.Users;

namespace backaramis.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<AddUser, User>().ReverseMap();
            CreateMap<EditUser, User>().ReverseMap();
            CreateMap<UserPerfil, PerfilModel>().ReverseMap();
            CreateMap<LoggModel, UserLog>().ReverseMap();

            CreateMap<Producto, ProductoInsert>().ReverseMap();
            CreateMap<Producto, ProductoUpdate>().ReverseMap();
            CreateMap<ProductoRubro, RubroDto>().ReverseMap();

            CreateMap<DocumentoDetalle, DocumentsDetallInsertDto>().ReverseMap();

            CreateMap<Documento, DocumentsUpdateDto>().ReverseMap();

            CreateMap<Recibo, ReciboInsertDto>().ReverseMap();
            CreateMap<Recibo, ReciboDto>().ReverseMap();

            CreateMap<ReciboDetalle, ReciboDetalleInsertDto>().ReverseMap();          
            CreateMap<ReciboDetalle, ReciboDetallDto>().ReverseMap();

            CreateMap<Cliente, ClienteDto>().ReverseMap();
            CreateMap<Cliente, ClienteInsert>().ReverseMap();
        }
    }
}