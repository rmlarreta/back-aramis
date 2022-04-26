namespace backaramis.Interfaces
{
    public interface ILoggService
    {
        void Log(string detalle, string modulo, string tipo, string operador);
    }
}
