using AutoMapper;
using backaramis.Models;
using backaramis.Modelsdtos.Clientes;
using backaramis.Modelsdtos.Commons;
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

            CreateMap<Cliente, ClienteDto>().ReverseMap();
            CreateMap<Cliente, ClienteInsert>().ReverseMap();
        }
    }
}