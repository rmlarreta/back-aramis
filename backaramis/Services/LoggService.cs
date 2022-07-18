using AutoMapper;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Commons;

namespace backaramis.Services
{
    public class LoggService : ILoggService
    {
        private readonly aramisContext _context;
        private readonly IMapper _mapper;
        public LoggService(IMapper mapper, aramisContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public void Log(string detalle, string modulo, string tipo, string operador)
        {
           // _context.ChangeTracker.Clear();

            LoggModel loggModel = new()
            {
                Detalle = detalle,
                Modulo = modulo,
                Tipo = tipo,
                Fecha = System.DateTime.Now.AddHours(-3),
                Operador = operador
            };
            UserLog? logg = _mapper.Map<UserLog>(loggModel);
            _context.UserLogs.Add(logg);
            _context.SaveChanges();
        }
    }
}
