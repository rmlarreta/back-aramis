namespace backaramis.Modelsdtos.Recibos
{
    public class EventoDto
    {
        public List<Evento>? Events { get; set; }

    }

    public class Evento
    {
        public string? Payment_intent_id { get; set; }
        public string? Status { get; set; }
        public string? Created_on { get; set; }
    }

}
